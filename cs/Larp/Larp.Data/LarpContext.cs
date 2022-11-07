using Larp.Data.Services;
using Larp.Protos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Larp.Data;

public class LarpContext
{
    public IMongoCollection<Data.Account> Accounts { get; }
    public IMongoCollection<Event> Events { get; }
    public FifthEditionContext FifthEdition { get; }
    public IMongoCollection<Game> Games { get; }
    public IMongoCollection<Session> Sessions { get; }

    public LarpContext(IOptions<LarpDataOptions> options, LarpDataCache cache)
    {
        var client = new MongoClient(options.Value.ConnectionString ?? throw new MongoConfigurationException("Connection String must be provided in options"));
        var database = client.GetDatabase(options.Value.Database ?? throw new MongoConfigurationException("Database must be provided in options"));
        Accounts = database.GetCollection<Account>(nameof(Accounts));
        Events = database.GetCollection<Event>(nameof(Events));
        Games = database.GetCollection<Game>(nameof(Games));
        Sessions = database.GetCollection<Session>(nameof(Sessions));
        FifthEdition = new FifthEditionContext(database, cache);
    }
}