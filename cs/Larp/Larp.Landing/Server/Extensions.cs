using System.Reflection;
using System.Text.Json;
using Larp.Landing.Shared;

namespace Larp.Landing.Server;

public static class Extensions
{
    public static void MapApi<TInterface>(this WebApplication app)
    {
        var root = typeof(TInterface).GetCustomAttribute<ApiRootAttribute>();
        if (root == null)
            throw new InvalidOperationException($"{nameof(ApiRootAttribute)} is required to call {nameof(MapApi)}");

        var rootUri = root.ApiRootPath.TrimEnd('/') + "/";

        var methods = typeof(TInterface).GetMethods()
            .Select(x => (MethodInfo: x, Api: x.GetCustomAttribute<ApiGet>()))
            .Where(x => x.Api != null);

        foreach (var (methodInfo, api) in methods)
        {
            if (api!.HttpMethod == HttpMethod.Get)
            {
                app.MapGet(rootUri + api.ApiPath,
                    async (HttpContext httpContext, TInterface obj) =>
                        await HandleRequest(httpContext, methodInfo, obj));
            }
            else if (api.HttpMethod == HttpMethod.Post)
            {
                app.MapPost(rootUri + api.ApiPath,
                    async (HttpContext httpContext, TInterface obj) =>
                        await HandleRequest(httpContext, methodInfo, obj));
            }
        }

    }
    
    static async Task HandleRequest<TInterface>(HttpContext httpContext, MethodInfo methodInfo, TInterface obj)
    {
        var body = httpContext.Request.HasJsonContentType()
            ? await JsonDocument.ParseAsync(httpContext.Request.Body)
            : null;
        
        var parameters= methodInfo.GetParameters()
            .Select(parameter =>
            {
                var name = parameter.Name!;
                
                if (httpContext.GetRouteData().Values.TryGetValue(name, out var routeValue))
                    return routeValue;
                if (body != null)
                    return body.RootElement.GetProperty(name).Deserialize(parameter.ParameterType);
                if (httpContext.Request.Query.TryGetValue(name, out var value))
                    return value.FirstOrDefault();
                
                throw new Exception($"Parameter {name} was not found in route or body");
            })
            .ToArray();
        
        var result = (dynamic)methodInfo.Invoke(obj, parameters)!;
        var response = await result;
        await httpContext.Response.WriteAsJsonAsync((object)response);
    }
}