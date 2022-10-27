using Larp.Data.Seeder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mongo2Go;

namespace Larp.Data.TextFixture;

public class LarpDataTestFixture : IDisposable
{
    public LarpContext Context { get; set; }

    private readonly MongoDbRunner _runner;

    public static async Task<LarpDataTestFixture> CreateTestFixtureAsync(bool seed)
    {
        var runner = MongoDbRunner.Start();

        var result = new LarpDataTestFixture(runner);

        if (seed)
        {
            var seederLogger = LoggerFactory.Create(null).CreateLogger<LarpDataSeeder>();
            var seeder = new LarpDataSeeder(result.Context, seederLogger);
            await seeder.Seed();
        }

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
        Context = new LarpContext(Options.Create(options));
    }
    
    public void Dispose()
    {
        _runner.Dispose();
    }
}