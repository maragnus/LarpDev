namespace Larp.Common.LifeCycle;

public interface IStartupTask
{
    Task StartAsync(CancellationToken cancellationToken);
}
