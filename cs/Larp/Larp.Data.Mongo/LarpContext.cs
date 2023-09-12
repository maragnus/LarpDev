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
        var connectionString =
            options.Value.ConnectionString ??
            throw new MongoConfigurationException(
                $"{LarpDataOptions.SectionName}.{nameof(LarpDataOptions.ConnectionString)} must be provided in options");
        var databaseName =
            options.Value.Database ??
            throw new MongoConfigurationException(
                $"{LarpDataOptions.SectionName}.{nameof(LarpDataOptions.Database)} must be provided in options");

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        AccountAttachments = database.GetCollection<AccountAttachment>(nameof(AccountAttachments));
        Accounts = database.GetCollection<Account>(nameof(Accounts));
        Attendances = database.GetCollection<Attendance>(nameof(Attendances));
        Citations = database.GetCollection<Citation>(nameof(Citations));
        ClarifyTerms = database.GetCollection<ClarifyTerm>(nameof(ClarifyTerms));
        EventLog = database.GetCollection<BsonDocument>(nameof(EventLog));
        Events = database.GetCollection<Event>(nameof(Events));
        GameStates = database.GetCollection<BsonDocument>(nameof(GameStates));
        Games = database.GetCollection<Game>(nameof(Games));
        LetterTemplates = database.GetCollection<LetterTemplate>(nameof(LetterTemplates));
        Letters = database.GetCollection<Letter>(nameof(Letters));
        MwFifthGame = new MwFifthGameContext(database, cache);
        Sessions = database.GetCollection<Session>(nameof(Sessions));
        Transactions = database.GetCollection<Transaction>(nameof(Transactions));
    }

    public IMongoCollection<Account> Accounts { get; }
    public IMongoCollection<AccountAttachment> AccountAttachments { get; }
    public IMongoCollection<Attendance> Attendances { get; }
    public IMongoCollection<BsonDocument> EventLog { get; }
    public IMongoCollection<BsonDocument> GameStates { get; }
    public IMongoCollection<Citation> Citations { get; }
    public IMongoCollection<ClarifyTerm> ClarifyTerms { get; }
    public IMongoCollection<Event> Events { get; }
    public IMongoCollection<Game> Games { get; }
    public IMongoCollection<Letter> Letters { get; }
    public IMongoCollection<LetterTemplate> LetterTemplates { get; }
    public IMongoCollection<Session> Sessions { get; }
    public IMongoCollection<Transaction> Transactions { get; set; }
    public MwFifthGameContext MwFifthGame { get; }

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