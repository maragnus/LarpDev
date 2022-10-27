using System.Threading;
using System.Threading.Tasks;

namespace Larp.Common.LifeCycle;

public interface IStopTask
{
    Task StopAsync(CancellationToken cancellationToken);
}
