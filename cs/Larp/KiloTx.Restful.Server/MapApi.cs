using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace KiloTx.Restful.Server;

public static class MapApiExtensions
{
    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase  
    };
    
    
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
                AuthRequired: x.GetCustomAttribute<ApiAuthenticatedAttribute>(),
                ContentType: x.GetCustomAttribute<ApiContentTypeAttribute>()))
            .Where(x => x.Api != null);

        foreach (var (methodInfo, api, authRequired, contentType) in methods)
        {
            var uri = rootUri + api!.ApiPath.Trim('/');

            logger.LogInformation("Map {Method} {Uri} for {Type}", api.HttpMethod, uri, typeof(TInterface).Name);

            app.MapMethods(uri, new[] { api.HttpMethod.Method },
                async (HttpContext httpContext, TInterface obj) =>
                    await HandleRequest(httpContext, methodInfo, obj, logger, authRequired, contentType));
        }
    }

    static async Task HandleRequest<TInterface>(HttpContext httpContext, MethodInfo methodInfo, TInterface obj,
        ILogger logger, ApiAuthenticatedAttribute? authRequired,
        ApiContentTypeAttribute? contentType)
    {
        if (authRequired != null)
        {
            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                // 401 Unauthorized is the status code to return when the client provides no credentials or invalid credentials.
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsJsonAsync(Result.Failed("Must be logged in to access this section"),
                    JsonOptions);
                return;
            }

            if (authRequired.Roles.Length > 0 && !authRequired.Roles.Any(role => httpContext.User.IsInRole(role)))
            {
                // 401 Unauthorized is the status code to return when the client provides no credentials or invalid credentials.
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsJsonAsync(Result.Failed($"You do not have the required role of {string.Join(" or ", authRequired.Roles)}"),
                    JsonOptions);
                return;
            }
        }

        try
        {
            var body = httpContext.Request.HasJsonContentType()
                ? await JsonSerializer.DeserializeAsync<JsonDocument>(httpContext.Request.Body, JsonOptions)
                : null;

            var form = httpContext.Request.HasFormContentType
                ? await httpContext.Request.ReadFormAsync()
                : null;

            var file = form?.Files.FirstOrDefault();

            var parameters = methodInfo.GetParameters()
                .Select(parameter =>
                {
                    var name = parameter.Name!;

                    if (file != null && parameter.Name == "fileName")
                        return file.FileName;
                    if (file != null && parameter.Name == "mediaType")
                        return file.ContentType;
                    if (parameter.ParameterType == typeof(Stream))
                        return form?.Files.FirstOrDefault()?.OpenReadStream() ?? throw new BadRequestException($"Parameter {name} only accepts one file");
                    if (httpContext.GetRouteData().Values.TryGetValue(name, out var routeValue))
                        return Coerce(routeValue, parameter.ParameterType);
                    if (httpContext.Request.Query.TryGetValue(name, out var value))
                        return Coerce(value.FirstOrDefault(), parameter.ParameterType);
                    if (body?.RootElement.TryGetProperty(name, out var property) == true)
                        return property.Deserialize(parameter.ParameterType, JsonOptions);
                    if (form?.TryGetValue(name, out value) == true)
                        return Coerce(value.FirstOrDefault(), parameter.ParameterType);
                    
                    throw new BadRequestException($"Parameter {name} was not found in route or body");
                })
                .ToArray();

            var result = (dynamic)methodInfo.Invoke(obj, parameters)!;

            if (methodInfo.ReturnType == typeof(Task<IFileInfo>))
            {
                if (contentType != null)
                    httpContext.Response.ContentType = contentType.ContentType;
                var fileInfo = await (Task<IFileInfo>)result;
                await httpContext.Response.SendFileAsync(fileInfo);
                if (!string.IsNullOrEmpty( fileInfo.PhysicalPath) && File.Exists(fileInfo.PhysicalPath))
                    File.Delete(fileInfo.PhysicalPath!);
                return;
            }

            if (methodInfo.ReturnType == typeof(Task))
            {
                await (Task)result;
                return;
            }

            var response = await result;
            await httpContext.Response.WriteAsJsonAsync((object)response, JsonOptions);
        }
        catch (BadRequestException ex)
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

    private static object? Coerce(object? value, Type desiredType)
    {
        if (value is not string stringValue)
            return Convert.ChangeType(value, desiredType);

        if (desiredType == typeof(string))
            return value;

        if (desiredType.IsEnum)
        {
            if (Enum.TryParse(desiredType, stringValue, true, out var enumValue))
                return enumValue;
            throw new BadRequestException($"Value \"{value}\" does not map to enum \"{desiredType.Name}\"");
        }

        return Convert.ChangeType(value, desiredType);
    }
}