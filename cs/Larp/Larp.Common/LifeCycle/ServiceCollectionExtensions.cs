using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Larp.Common.LifeCycle;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Construct <typeparamref name="TStartupTask"/> through Dependency Injection and executes <see cref="IStartupTask.StartAsync(CancellationToken)"/> during WebHost startup
    /// </summary>
    public static IServiceCollection AddStartupTask<TStartupTask>(this IServiceCollection serviceCollection)
       where TStartupTask : class, IStartupTask
    {
        var lists = GetTaskList(serviceCollection);
        serviceCollection.AddScoped<TStartupTask>();
        lists.StartupTypes.Add(typeof(TStartupTask));
        return serviceCollection;
    }

    /// <summary>
    /// Executes <see cref="IStartupTask.StartAsync(CancellationToken)"/> on <typeparamref name="TStartupTask"/> during WebHost startup
    /// </summary>
    public static IServiceCollection AddStartupTask<TStartupTask>(this IServiceCollection serviceCollection, TStartupTask startupTask)
        where TStartupTask : class, IStartupTask
    {
        var lists = GetTaskList(serviceCollection);
        lists.StartupInstances.Add(startupTask);
        return serviceCollection;
    }

    /// <summary>
    /// Executes <paramref name="action"/> during WebHost startup
    /// </summary>
    public static IServiceCollection AddStartupTask(this IServiceCollection serviceCollection, Func<CancellationToken, Task> action, [CallerMemberName] string? name = null!)
    {
        var lists = GetTaskList(serviceCollection);
        lists.StartupActions.Add((name ?? "Unnamed task", action));
        return serviceCollection;
    }

    /// <summary>
    /// Construct <typeparamref name="TStopTask"/> through Dependency Injection and executes <see cref="IStopTask.StopAsync(CancellationToken)"/> during WebHost stop
    /// </summary>
    public static IServiceCollection AddStopTask<TStopTask>(this IServiceCollection serviceCollection)
       where TStopTask : class, IStopTask
    {
        var lists = GetTaskList(serviceCollection);
        serviceCollection.AddScoped<TStopTask>();
        lists.StopTypes.Add(typeof(TStopTask));
        return serviceCollection;
    }

    /// <summary>
    /// Executes <see cref="IStopTask.StopAsync(CancellationToken)"/> on <typeparamref name="TStopTask"/> during WebHost stop
    /// </summary>
    public static IServiceCollection AddStopTask<TStopTask>(this IServiceCollection serviceCollection, TStopTask stopTask)
        where TStopTask : class, IStopTask
    {
        var lists = GetTaskList(serviceCollection);
        lists.StopInstances.Add(stopTask);
        return serviceCollection;
    }

    /// <summary>
    /// Executes <paramref name="action"/> during WebHost stop
    /// </summary>
    public static IServiceCollection AddStopTask(this IServiceCollection serviceCollection, Func<CancellationToken, Task> action, [CallerMemberName] string? name = null!)
    {
        var lists = GetTaskList(serviceCollection);
        lists.StopActions.Add((name ?? "Unnamed task", action));
        return serviceCollection;
    }

    private static LifecycleTaskService.LifeCycleTasksLists GetTaskList(IServiceCollection serviceCollection)
    {
        var lists = serviceCollection.FirstOrDefault(x => x.ImplementationInstance is LifecycleTaskService.LifeCycleTasksLists)?
            .ImplementationInstance as LifecycleTaskService.LifeCycleTasksLists;
        
        if (lists == null)
        {
            lists = new LifecycleTaskService.LifeCycleTasksLists();
            serviceCollection.AddSingleton(lists);
            serviceCollection.AddHostedService<LifecycleTaskService>();
        }

        return lists;
    }
}
