﻿using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Data.Mongo;

public class LarpContext
{
    static LarpContext()
    {
        // var enumConvention = new ConventionPack() { new EnumRepresentationConvention(BsonType.String) };
        // ConventionRegistry.Register(nameof(EnumRepresentationConvention), enumConvention, _ => true);
    }

    public LarpContext(IOptions<LarpDataOptions> options, LarpDataCache cache, ILogger<LarpContext> logger)
    {
        var client = new MongoClient(options.Value.ConnectionString ??
                                     throw new MongoConfigurationException(
                                         "Connection String must be provided in options"));
        var database = client.GetDatabase(options.Value.Database ??
                                          throw new MongoConfigurationException(
                                              "Database must be provided in options"));

        Accounts = database.GetCollection<Account>(nameof(Accounts));
        Attendances = database.GetCollection<Attendance>(nameof(Attendances));
        Events = database.GetCollection<Event>(nameof(Events));
        Games = database.GetCollection<Game>(nameof(Games));
        Sessions = database.GetCollection<Session>(nameof(Sessions));
        GameStates = database.GetCollection<BsonDocument>(nameof(GameStates));
        MwFifthGame = new MwFifthGameContext(database, cache);
    }

    public IMongoCollection<Account> Accounts { get; }
    public IMongoCollection<Event> Events { get; }
    public IMongoCollection<Attendance> Attendances { get; }
    public MwFifthGameContext MwFifthGame { get; }
    public IMongoCollection<Game> Games { get; }
    public IMongoCollection<Session> Sessions { get; }
    public IMongoCollection<BsonDocument> GameStates { get; }

    public async Task Migrate()
    {
        await MigrateToCharacterRevisions();
        await FixCharacters();
        await CreateIndices();
    }

    private async Task CreateIndices()
    {
        await Sessions.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Session>(Builders<Session>.IndexKeys.Ascending(x => x.AccountId)),
        });

        await Attendances.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Attendance>(Builders<Attendance>.IndexKeys.Ascending(x => x.AccountId)),
            new CreateIndexModel<Attendance>(Builders<Attendance>.IndexKeys.Ascending(x => x.EventId)),
        });

        await MwFifthGame.Characters.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Character>(Builders<Character>.IndexKeys.Ascending(x => x.AccountId)),
            new CreateIndexModel<Character>(Builders<Character>.IndexKeys.Ascending(x => x.UniqueId)),
        });

        await MwFifthGame.CharacterRevisions.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<CharacterRevision>(Builders<CharacterRevision>.IndexKeys.Ascending(x => x.AccountId)),
            new CreateIndexModel<CharacterRevision>(Builders<CharacterRevision>.IndexKeys.Ascending(x => x.UniqueId))
        });
    }

    private async Task FixCharacters()
    {
        var characters = (await MwFifthGame.Characters
                .Find(_ => true)
                .ToListAsync())
            .ToDictionary(x => x.UniqueId);

        var revisions = await MwFifthGame.CharacterRevisions
            .Find(x => x.State == CharacterState.Live)
            .ToListAsync();

        foreach (var revision in revisions)
        {
            var character = characters[revision.UniqueId];
            var usedMoonstone = revision.SkillMoonstone + revision.GiftMoonstone;
            if (character.CharacterName == revision.CharacterName && usedMoonstone == character.UsedMoonstone)
                continue;

            await MwFifthGame.Characters.UpdateOneAsync(
                characterFilter => characterFilter.UniqueId == revision.UniqueId,
                Builders<Character>.Update
                    .Set(x => x.CharacterName, revision.CharacterName)
                    .Set(x => x.UsedMoonstone, usedMoonstone));
        }
    }

    private async Task MigrateToCharacterRevisions()
    {
        var count = await MwFifthGame.Characters.CountDocumentsAsync(_ => true);
        if (count == 0)
        {
            var revisions = await MwFifthGame.CharacterRevisions.Find(_ => true).ToListAsync();
            var ids = revisions.GroupBy(x => x.UniqueId);
            foreach (var id in ids)
            {
                var first = id.First();
                await MwFifthGame.Characters.InsertOneAsync(new Character()
                {
                    UniqueId = first.UniqueId,
                    AccountId = first.AccountId,
                    CreatedOn = first.CreatedOn
                });
            }
        }
    }
}