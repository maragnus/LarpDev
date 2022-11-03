namespace Larp.Common.LifeCycle;

public interface IStopTask
{
    Task StopAsync(CancellationToken cancellationToken);
}
