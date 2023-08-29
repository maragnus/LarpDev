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
        ClarifyTerms = database.GetCollection<ClarifyTerm>(nameof(ClarifyTerms));
        Sessions = database.GetCollection<Session>(nameof(Sessions));
        GameStates = database.GetCollection<BsonDocument>(nameof(GameStates));
        Letters = database.GetCollection<Letter>(nameof(Letters));
        LetterTemplates = database.GetCollection<LetterTemplate>(nameof(LetterTemplates));
        MwFifthGame = new MwFifthGameContext(database, cache);
        EventLog = database.GetCollection<BsonDocument>(nameof(EventLog));
        Citations = database.GetCollection<Citation>(nameof(Citations));
    }

    public IMongoCollection<BsonDocument> EventLog { get; }
    public IMongoCollection<AccountAttachment> AccountAttachments { get; }
    public IMongoCollection<Account> Accounts { get; }
    public IMongoCollection<Event> Events { get; }
    public IMongoCollection<Attendance> Attendances { get; }
    public MwFifthGameContext MwFifthGame { get; }
    public IMongoCollection<Game> Games { get; }
    public IMongoCollection<Session> Sessions { get; }
    public IMongoCollection<BsonDocument> GameStates { get; }
    public IMongoCollection<Letter> Letters { get; }
    public IMongoCollection<LetterTemplate> LetterTemplates { get; }
    public IMongoCollection<ClarifyTerm> ClarifyTerms { get; }
    public IMongoCollection<Citation> Citations { get; }

    public async Task Migrate()
    {
        await CreateIndices();
        await UpdateMoonstone();
    }

    private async Task UpdateMoonstone()
    {
        await MwFifthCharacterManager.UpdateMoonstone(this);
    }

    public async ValueTask<string> GetGameIdByName(string gameName) =>
        await Games
            .Find(game => game.Name == gameName)
            .Project(game => game.Id)
            .FirstAsync();

    private async Task CreateIndices()
    {
        await Accounts.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Account>(Builders<Account>.IndexKeys.Ascending("Emails.NormalizedEmail")),
            new CreateIndexModel<Account>(Builders<Account>.IndexKeys.Ascending(x => x.State)),
        });

        await Sessions.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Session>(Builders<Session>.IndexKeys.Ascending(x => x.AccountId)),
        });

        await Letters.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Letter>(Builders<Letter>.IndexKeys.Ascending(x => x.AccountId)),
            new CreateIndexModel<Letter>(Builders<Letter>.IndexKeys.Ascending(x => x.EventId)),
        });

        await Attendances.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Attendance>(Builders<Attendance>.IndexKeys.Ascending(x => x.AccountId)),
            new CreateIndexModel<Attendance>(Builders<Attendance>.IndexKeys.Ascending(x => x.EventId)),
        });

        await MwFifthGame.Characters.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Character>(Builders<Character>.IndexKeys.Ascending(x => x.State)),
            new CreateIndexModel<Character>(Builders<Character>.IndexKeys.Ascending(x => x.AccountId))
        });

        await MwFifthGame.CharacterRevisions.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<CharacterRevision>(Builders<CharacterRevision>.IndexKeys.Ascending(x => x.State)),
            new CreateIndexModel<CharacterRevision>(Builders<CharacterRevision>.IndexKeys.Ascending(x => x.AccountId)),
            new CreateIndexModel<CharacterRevision>(Builders<CharacterRevision>.IndexKeys.Ascending(x => x.CharacterId))
        });

        await ClarifyTerms.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<ClarifyTerm>(Builders<ClarifyTerm>.IndexKeys.Ascending(x => x.GameId)
                .Ascending(x => x.Name)),
            new CreateIndexModel<ClarifyTerm>(Builders<ClarifyTerm>.IndexKeys.Ascending(x => x.Name)),
        });
    }

    public async Task LogEvent(string actorAccountId, string? actorSessionId, LogEvent logEvent)
    {
        logEvent.LogEventId = LarpContext.GenerateNewId();
        logEvent.ActorAccountId = actorAccountId;
        logEvent.ActorSessionId = actorSessionId;
        logEvent.ActedOn = DateTimeOffset.Now;
        await EventLog.InsertOneAsync(logEvent.ToBsonDocument());
    }

    public async Task LogEvent<TLogEvent>(string actorAccountId, string? actorSessionId, TLogEvent logEvent)
        where TLogEvent : LogEvent
    {
        logEvent.LogEventId = LarpContext.GenerateNewId();
        logEvent.ActorAccountId = actorAccountId;
        logEvent.ActorSessionId = actorSessionId;
        logEvent.ActedOn = DateTimeOffset.Now;
        await EventLog.InsertOneAsync(logEvent.ToBsonDocument());
    }

    public static string GenerateNewId() => ObjectId.GenerateNewId().ToString()!;
}