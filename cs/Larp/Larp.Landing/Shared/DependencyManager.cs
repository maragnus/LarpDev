using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Larp.Landing.Shared;

[PublicAPI]
public class DependencyManager<T>
    where T : class
{
    private readonly ILogger? _logger;
    private readonly Dictionary<string, HashSet<MethodInfo>> _dependencies;

    public DependencyManager(ILogger? logger)
    {
        _logger = logger;
        
        var methods = typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .DistinctBy(x=>x.Name)
            .ToDictionary(x => x.Name);

        var properties = ( 
            from prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            let dependsOn = prop.GetCustomAttribute<DependsOnAttribute>()
            where dependsOn != null
            select (prop.Name, dependsOn.PropertyNames, methods[dependsOn.UpdateMethod])).ToList();

        _dependencies = new Dictionary<string, HashSet<MethodInfo>>();

        foreach (var dependency in properties)
        {
            foreach (var property in dependency.PropertyNames)
            {
                if (!_dependencies.TryGetValue(property, out var set))
                    _dependencies[property] = set = new HashSet<MethodInfo>();
                set.Add(dependency.Item3);
            }
        }
    }

    public void Update(T parent, string memberName)
    {
        if (!_dependencies.TryGetValue(memberName, out var methods))
        {
            _logger?.LogInformation("Field {MemberName} updated but there's no dependencies", memberName);
            return;
        }

        var dependencies = methods.Distinct().ToList();

        _logger?.LogInformation("Field {MemberName} updated, triggering: {Dependencies}",
            memberName, string.Join(", ", dependencies.Select(x => x.Name)));
        
        foreach (var method in dependencies)
            method.Invoke(parent, Array.Empty<object>());
    }

    public void UpdateAll(T parent)
    {
        foreach (var method in _dependencies.Values.SelectMany(x => x).Distinct())
            method.Invoke(parent, Array.Empty<object>());
    }
}