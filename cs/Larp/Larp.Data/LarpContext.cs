using Larp.Data.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Larp.Data;

public class LarpContext
{
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
        FifthEdition = new FifthEditionContext(database, cache);
    }

    public IMongoCollection<Account> Accounts { get; }
    public IMongoCollection<Event> Events { get; }
    public IMongoCollection<Attendance> Attendances { get; }
    public FifthEditionContext FifthEdition { get; }
    public IMongoCollection<Game> Games { get; }
    public IMongoCollection<Session> Sessions { get; }
}