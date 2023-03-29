﻿using System.Reflection;
using System.Text.Json;
using Larp.Landing.Server.Services;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;

namespace Larp.Landing.Server;
public class ResourceForbiddenException : Exception
{
    public ResourceForbiddenException() : base("User is authenticated but not privileged to access this resource")
    {
    }
}

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException() : base("Resource was not found")
    {
    }
}

public static class MapApiExtensions
{
    public static void MapApi<TInterface>(this WebApplication app)
        where TInterface : class
    {
        var logger = app.Services.GetRequiredService<ILogger<TInterface>>();

        var root = typeof(TInterface).GetCustomAttribute<ApiRootAttribute>();
        if (root == null)
            throw new InvalidOperationException($"{nameof(ApiRootAttribute)} is required to call {nameof(MapApi)}");

        var rootUri = $"{root.ApiRootPath.Trim('/')}/";

        var methods = typeof(TInterface).GetMethods()
            .Select(x => (
                MethodInfo: x,
                Api: x.GetCustomAttribute<ApiRouteAttribute>(),
                AuthRequired: x.GetCustomAttribute<ApiAuthenticatedAttribute>() != null))
            .Where(x => x.Api != null);

        foreach (var (methodInfo, api, authRequired) in methods)
        {
            var uri = rootUri + api!.ApiPath.Trim('/');

            logger.LogInformation("Map {Method} {Uri} for {Type}", api.HttpMethod, uri, typeof(TInterface).Name);

            app.MapMethods(uri, new [] { api.HttpMethod.Method },
                async (HttpContext httpContext, TInterface obj, IUserSession session) =>
                    await HandleRequest(httpContext, methodInfo, obj, logger, session, authRequired));
        }
    }

    static async Task HandleRequest<TInterface>(HttpContext httpContext, MethodBase methodInfo, TInterface obj,
        ILogger logger,
        IUserSession userSession, bool authRequired)
    {
        if (authRequired && !userSession.IsAuthenticated)
        {
            // 401 Unauthorized is the status code to return when the client provides no credentials or invalid credentials.
            httpContext.Response.StatusCode = 401;
            await httpContext.Response.WriteAsJsonAsync(Result.Failed("Must be logged in to access this section"), LarpJson.Options);
            return;
        }

        try
        {
            var body = httpContext.Request.HasJsonContentType()
                ? await JsonSerializer.DeserializeAsync<JsonDocument>(httpContext.Request.Body, LarpJson.Options)
                : null;

            var parameters = methodInfo.GetParameters()
                .Select(parameter =>
                {
                    var name = parameter.Name!;

                    if (httpContext.GetRouteData().Values.TryGetValue(name, out var routeValue))
                        return routeValue;
                    if (httpContext.Request.Query.TryGetValue(name, out var value))
                        return value.FirstOrDefault();
                    if (body?.RootElement.TryGetProperty(name, out var property) == true)
                        return property.Deserialize(parameter.ParameterType, LarpJson.Options);

                    throw new BadHttpRequestException($"Parameter {name} was not found in route or body");
                })
                .ToArray();

            var result = (dynamic)methodInfo.Invoke(obj, parameters)!;
            var response = await result;
            await httpContext.Response.WriteAsJsonAsync((object)response, LarpJson.Options);
        }
        catch (BadHttpRequestException ex)
        {
            // 400 Bad Request is the status code to return when the form of the client request is not as the API expects.
            logger.LogWarning(ex, "Request is not expected: {Uri}", httpContext.Request.Path);
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync(ex.ToString());
        }
        catch (ResourceNotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found: {Uri}", httpContext.Request.Path);
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsync(ex.ToString());
        }     
        catch (ResourceForbiddenException ex)
        {
            // 403 Forbidden is the status code to return when a client has valid credentials but not enough privileges to perform an action on a resource.
            logger.LogWarning(ex, "Access to resource is forbidden:  {Uri}", httpContext.Request.Path);
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync(ex.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error rendering {Uri}: {Message}", httpContext.Request.Path, ex.Message);
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync(ex.ToString());
        }
    }
}