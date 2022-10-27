using Larp.Common.LifeCycle;

namespace Larp.Data.Seeder;

public sealed class LarpDataSeederStartupTask : IStartupTask
{
    private readonly LarpDataSeeder _dataSeeder;

    public LarpDataSeederStartupTask(LarpDataSeeder dataSeeder)
    {
        _dataSeeder = dataSeeder;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dataSeeder.Seed();
    }
}