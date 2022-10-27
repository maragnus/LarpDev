using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Larp.Common.LifeCycle;

namespace Larp.Common.LifeCycle;

public class LifecycleTaskService : IHostedService
{
    private readonly ILogger<LifecycleTaskService> _logger;
    private readonly LifeCycleTasksLists _taskList;
    private readonly IServiceProvider _serviceProvider;

    public class LifeCycleTasksLists
    {
        public List<(string Name, Func<CancellationToken, Task> Action)> StartupActions { get; } = new();
        public List<Type> StartupTypes { get; } = new();
        public List<IStartupTask> StartupInstances { get; } = new();


        public List<(string Name, Func<CancellationToken, Task> Action)> StopActions { get; } = new();
        public List<Type> StopTypes { get; } = new();
        public List<IStopTask> StopInstances { get; } = new();
    }

    public LifecycleTaskService(ILogger<LifecycleTaskService> logger, LifeCycleTasksLists startupTaskList, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _taskList = startupTaskList;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var task in _taskList.StartupActions)
        {
            _logger.LogInformation("Executing action {StartupActionName} startup task", task.Name);
            await task.Action(cancellationToken);
        }

        foreach (var task in _taskList.StartupInstances)
        {
            _logger.LogInformation("Executing instance {StartupTaskType} startup task", task.GetType().FullName);
            await task.StartAsync(cancellationToken);
        }

        foreach (var type in _taskList.StartupTypes)
        {
            _logger.LogInformation("Executing scoped dependency injected {StartupTaskType} startup task", type.FullName);
            await using var scope = _serviceProvider.CreateAsyncScope();
            var task = (IStartupTask)scope.ServiceProvider.GetRequiredService(type);
            await task.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var task in _taskList.StopActions)
        {
            _logger.LogInformation("Executing action {StopActionName} stop task", task.Name);
            await task.Action(cancellationToken);
        }

        foreach (var task in _taskList.StopInstances)
        {
            _logger.LogInformation("Executing instance {StopTaskType} stop task", task.GetType().FullName);
            await task.StopAsync(cancellationToken);
        }

        foreach (var type in _taskList.StopTypes)
        {
            _logger.LogInformation("Executing scoped dependency injected {StopTaskType} stop task", type.FullName);
            await using var scope = _serviceProvider.CreateAsyncScope();
            var task = (IStopTask)scope.ServiceProvider.GetRequiredService(type);
            await task.StopAsync(cancellationToken);
        }
    }
}
