using System.Reflection;
using Google.Protobuf;

namespace Larp.WebService.ProtobufControllers;

public record ProtobufControllerInfo(
    Type ControllerType,
    MethodInfo MethodInfo,
    Type? RequestMessageType,
    Type? ResponseMessageType,
    bool HasRequestMessage,
    bool HasResponseMessage,
    bool IsAsync
);

public class ProtobufControllerMap
{
    private static readonly Type MessageType = typeof(IMessage);
    private readonly Dictionary<string, ProtobufControllerTypeInfo> _controllers = new();

    public void Add<TController>() where TController : ProtobufController
    {
        var name = typeof(TController).Name;
        if (name.EndsWith("Controller"))
            name = name[..^"Controller".Length];

        var methods = FindMethods<TController>();

        var info = new ProtobufControllerTypeInfo(typeof(TController), methods);

        _controllers.Add(name, info);
    }

    private static (Type?, bool isTask) GetMessageType(Type type)
    {
        if (MessageType.IsAssignableFrom(type))
            return (type, false);

        if (type == typeof(Task) || type == typeof(ValueTask))
            return (null, true);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var childType = type.GetGenericArguments()[0];
            if (MessageType.IsAssignableFrom(childType))
                return (childType, true);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            var childType = type.GetGenericArguments()[0];
            if (MessageType.IsAssignableFrom(childType))
                return (childType, true);
        }

        return (null, false);
    }

    private static Dictionary<string, ProtobufControllerMethodInfo> FindMethods<TController>()
        where TController : ProtobufController
    {
        var result = new Dictionary<string, ProtobufControllerMethodInfo>();
        var methods = typeof(TController).GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var method in methods)
        {
            var name = method.Name;
            var (returnType, isTask) = GetMessageType(method.ReturnType);
            var parameters = method.GetParameters();
            var parameterType =
                parameters.Length > 0
                && MessageType.IsAssignableFrom(parameters[0].ParameterType)
                    ? parameters[0].ParameterType
                    : null;

            if (returnType == null && parameterType == null)
                continue;

            result[name] = new ProtobufControllerMethodInfo(
                method,
                returnType,
                parameterType,
                parameterType != null,
                returnType != null,
                isTask
            );
        }

        return result;
    }

    public ProtobufControllerInfo? Get(string controller, string method)
    {
        if (!_controllers.TryGetValue(controller, out var info))
            return null;

        if (!info.Methods.TryGetValue(method, out var methodInfo))
            return null;

        return new ProtobufControllerInfo(
            info.ControllerType,
            methodInfo.MethodInfo,
            methodInfo.RequestMessageType,
            methodInfo.ResponseMessageType,
            methodInfo.HasRequestMessage,
            methodInfo.HasResponseMessage,
            methodInfo.IsAsync);
    }

    private record ProtobufControllerTypeInfo(
        Type ControllerType,
        Dictionary<string, ProtobufControllerMethodInfo> Methods);

    private record ProtobufControllerMethodInfo(
        MethodInfo MethodInfo,
        Type? ResponseMessageType,
        Type? RequestMessageType,
        bool HasRequestMessage,
        bool HasResponseMessage,
        bool IsAsync);
}

public static class ProtobufControllerExtensions
{
    public static IServiceCollection AddProtobufController<TController>(this IServiceCollection serviceCollection)
        where TController : ProtobufController
    {
        if (serviceCollection
                .FirstOrDefault(x => x.ServiceType == typeof(ProtobufControllerMap))?
                .ImplementationInstance is not ProtobufControllerMap map)
        {
            map = new ProtobufControllerMap();
            serviceCollection.AddSingleton(map);
        }

        map.Add<TController>();
        serviceCollection.AddScoped<TController>();
        return serviceCollection;
    }
}