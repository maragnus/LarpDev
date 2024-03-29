﻿using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Data.Seeder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
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
            new LarpDataCache(Options.Create(new MemoryCacheOptions())));
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

        var seederLogger = NullLogger<LarpDataSeeder>.Instance;
        var seeder = new LarpDataSeeder(result.Context, seederLogger, clock);
        await seeder.SeedIfNeeded();

        return result;
    }
}