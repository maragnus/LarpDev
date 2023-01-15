using Larp.Data.Seeder;
using Larp.Data.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Mongo2Go;

namespace Larp.Data.TestFixture;

public class LarpDataTestFixture : IDisposable
{
    private readonly MongoDbRunner _runner;

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
            new LarpDataCache(Options.Create(new MemoryCacheOptions())),
            NullLogger<LarpContext>.Instance);
    }

    public LarpContext Context { get; set; }

    public void Dispose()
    {
        _runner.Dispose();
    }

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
}