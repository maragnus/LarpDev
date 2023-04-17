namespace KiloTx.Restful;

[AttributeUsage(AttributeTargets.Method)]
public class ApiAuthenticatedAttribute : Attribute
{
    public string[] Roles { get; }

    public ApiAuthenticatedAttribute(params string[] roles)
    {
        Roles = roles;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiContentTypeAttribute : Attribute
{
    public string ContentType { get; }

    public ApiContentTypeAttribute(string contentType)
    {
        ContentType = contentType;
    }
}

public abstract class ApiRouteAttribute : Attribute
{
    protected internal ApiRouteAttribute(HttpMethod httpMethod, string apiPath)
    {
        HttpMethod = httpMethod;
        ApiPath = apiPath;
    }

    public HttpMethod HttpMethod { get; }
    public string ApiPath { get; }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiGetAttribute : ApiRouteAttribute
{
    public ApiGetAttribute([RouteTemplate]string apiPath) : base(HttpMethod.Get, apiPath)
    { }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiPostAttribute : ApiRouteAttribute
{
    public ApiPostAttribute([RouteTemplate]string apiPath) : base(HttpMethod.Post, apiPath)
    { }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiDeleteAttribute : ApiRouteAttribute
{
    public ApiDeleteAttribute([RouteTemplate]string apiPath) : base(HttpMethod.Delete, apiPath)
    { }
}

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public sealed class ApiRootAttribute : Attribute
{
    public string ApiRootPath { get; }
    
    public ApiRootAttribute(string apiRootPath)
    {
        ApiRootPath = apiRootPath;
    }
}