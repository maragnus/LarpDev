using Larp.Data.Seeder;
using Larp.Data.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Mongo2Go;

namespace Larp.Data.TextFixture;

public class LarpDataTestFixture : IDisposable
{
    public LarpContext Context { get; set; }

    private readonly MongoDbRunner _runner;

    public static async Task<LarpDataTestFixture> CreateTestFixtureAsync(bool seed, ISystemClock clock)
    {
        var runner = MongoDbRunner.Start();

        var result = new LarpDataTestFixture(runner);

        if (!seed) return result;
        
        var seederLogger = LoggerFactory.Create(null).CreateLogger<LarpDataSeeder>();
        var seeder = new LarpDataSeeder(result.Context, seederLogger, clock);
        await seeder.Seed();

        return result;
    }

    private LarpDataTestFixture(MongoDbRunner runner)
    {
        _runner = runner;
        
        var options = new LarpDataOptions()
        {
            ConnectionString = runner.ConnectionString,
            Database = Guid.NewGuid().ToString()
        };
        Context = new LarpContext(
            Options.Create(options), 
            new LarpDataCache(Options.Create(new MemoryCacheOptions())));
    }
    
    public void Dispose()
    {
        _runner.Dispose();
    }
}