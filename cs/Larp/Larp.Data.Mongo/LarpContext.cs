using Larp.Data.Mongo.Services;
using Larp.Data.MwFifth;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;

namespace Larp.Data.Mongo;

public class LarpContext
{
    static LarpContext()
    {
        ConventionRegistry.Register("IgnoreIfDefault",
            new ConventionPack { new IgnoreIfDefaultConvention(true) },
            _ => true);
    }

    public LarpContext(IOptions<LarpDataOptions> options, LarpDataCache cache)
    {
        var client = new MongoClient(options.Value.ConnectionString ??
                                     throw new MongoConfigurationException(
                                         "Connection String must be provided in options"));
        var database = client.GetDatabase(options.Value.Database ??
                                          throw new MongoConfigurationException(
                                              "Database must be provided in options"));

        Accounts = database.GetCollection<Account>(nameof(Accounts));
        AccountAttachments = database.GetCollection<AccountAttachment>(nameof(AccountAttachments));
        Attendances = database.GetCollection<Attendance>(nameof(Attendances));
        Events = database.GetCollection<Event>(nameof(Events));
        Games = database.GetCollection<Game>(nameof(Games));
        Sessions = database.GetCollection<Session>(nameof(Sessions));
        GameStates = database.GetCollection<BsonDocument>(nameof(GameStates));
        Letters = database.GetCollection<Letter>(nameof(Letters));
        LetterTemplates = database.GetCollection<LetterTemplate>(nameof(LetterTemplates));
        MwFifthGame = new MwFifthGameContext(database, cache);
        EventLog = database.GetCollection<BsonDocument>(nameof(EventLog));
    }

    public IMongoCollection<BsonDocument> EventLog { get; set; }
    public IMongoCollection<AccountAttachment> AccountAttachments { get; set; }
    public IMongoCollection<Account> Accounts { get; }
    public IMongoCollection<Event> Events { get; }
    public IMongoCollection<Attendance> Attendances { get; }
    public MwFifthGameContext MwFifthGame { get; }
    public IMongoCollection<Game> Games { get; }
    public IMongoCollection<Session> Sessions { get; }
    public IMongoCollection<BsonDocument> GameStates { get; }
    public IMongoCollection<Letter> Letters { get; }
    public IMongoCollection<LetterTemplate> LetterTemplates { get; }

    public async Task Migrate()
    {
        await ActivateAccounts();
        await FixAccounts();
        await MigrateToCharacterRevisions();
        await FixCharacters();
        await CreateIndices();
    }

    private async Task FixAccounts()
    {
        var accounts = await Accounts.Find(_ => true).ToListAsync();
        foreach (var account in accounts.Where(x => x.Emails.Any(x => x.Email.Contains(" "))))
        {
            foreach (var email in account.Emails)
            {
                email.Email = email.Email.Replace(" ", "");
                email.NormalizedEmail = email.Email.ToLowerInvariant();
            }

            await Accounts.UpdateOneAsync(a => a.AccountId == account.AccountId,
                Builders<Account>.Update.Set(a => a.Emails, account.Emails));
        }
    }

    private async Task ActivateAccounts()
    {
        await Accounts.UpdateManyAsync(
            x => 
                x.State != AccountState.Active 
                && x.State != AccountState.Archived 
                && x.State != AccountState.Uninvited,
            Builders<Account>.Update.Set(x => x.State, AccountState.Active));
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
            new CreateIndexModel<Character>(Builders<Character>.IndexKeys.Ascending(x => x.CharacterId)),
        });

        await MwFifthGame.CharacterRevisions.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<CharacterRevision>(Builders<CharacterRevision>.IndexKeys.Ascending(x => x.AccountId)),
            new CreateIndexModel<CharacterRevision>(Builders<CharacterRevision>.IndexKeys.Ascending(x => x.CharacterId))
        });
    }

    private async Task FixCharacters()
    {
        var characters = (await MwFifthGame.Characters
                .Find(_ => true)
                .ToListAsync())
            .ToDictionary(x => x.CharacterId);

        var revisions = await MwFifthGame.CharacterRevisions
            .Find(x => x.State == CharacterState.Live)
            .ToListAsync();

        foreach (var revision in revisions)
        {
            var character = characters[revision.CharacterId];
            var usedMoonstone = revision.SkillMoonstone + revision.GiftMoonstone;
            if (character.CharacterName == revision.CharacterName && usedMoonstone == character.UsedMoonstone)
                continue;

            await MwFifthGame.Characters.UpdateOneAsync(
                characterFilter => characterFilter.CharacterId == revision.CharacterId,
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
            var ids = revisions.GroupBy(x => x.CharacterId);
            foreach (var id in ids)
            {
                var first = id.First();
                await MwFifthGame.Characters.InsertOneAsync(new Character()
                {
                    CharacterId = first.CharacterId,
                    AccountId = first.AccountId,
                    CreatedOn = first.CreatedOn
                });
            }
        }
    }

    public async Task LogEvent(string actorAccountId, string? actorSessionId, LogEvent logEvent)
    {
        logEvent.LogEventId = ObjectId.GenerateNewId().ToString();
        logEvent.ActorAccountId = actorAccountId;
        logEvent.ActorSessionId = actorSessionId;
        logEvent.ActedOn = DateTimeOffset.Now;
        await EventLog.InsertOneAsync(logEvent.ToBsonDocument());
    }
    
    public async Task LogEvent<TLogEvent>(string actorAccountId, string? actorSessionId, TLogEvent logEvent)
        where TLogEvent : LogEvent
    {
        logEvent.LogEventId = ObjectId.GenerateNewId().ToString();
        logEvent.ActorAccountId = actorAccountId;
        logEvent.ActorSessionId = actorSessionId;
        logEvent.ActedOn = DateTimeOffset.Now;
        await EventLog.InsertOneAsync(logEvent.ToBsonDocument());
    }
}