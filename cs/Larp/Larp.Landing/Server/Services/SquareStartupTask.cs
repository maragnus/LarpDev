using Larp.Common.LifeCycle;

namespace Larp.Landing.Server.Services;

public class SquareStartupTask : IStartupTask
{
    private readonly TransactionManager _transactionManager;

    public SquareStartupTask(TransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _transactionManager.SynchronizeOnStartup();
    }
}