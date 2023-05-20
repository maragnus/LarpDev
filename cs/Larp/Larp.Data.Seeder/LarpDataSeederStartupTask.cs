using Larp.Common.LifeCycle;
using Larp.Data.Mongo;

namespace Larp.Data.Seeder;

public sealed class LarpDataSeederStartupTask : IStartupTask
{
    private readonly LarpDataSeeder _dataSeeder;
    private readonly LarpContext _larpContext;

    public LarpDataSeederStartupTask(LarpDataSeeder dataSeeder, LarpContext larpContext)
    {
        _dataSeeder = dataSeeder;
        _larpContext = larpContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dataSeeder.SeedIfNeeded();
        await _larpContext.Migrate();
    }
}