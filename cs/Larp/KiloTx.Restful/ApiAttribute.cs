namespace KiloTx.Restful;

/// <summary>Require authenticated user hold one of the roles in <see cref="Roles"/> to access the method.</summary>
[AttributeUsage(AttributeTargets.Method)]
public class ApiAuthenticatedAttribute : Attribute
{
    public ApiAuthenticatedAttribute(params string[] roles)
    {
        Roles = roles;
    }

    public string[] Roles { get; }
}

/// <summary>Apply the Content Type of <see cref="ContentType"/> to the response.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiContentTypeAttribute : Attribute
{
    public ApiContentTypeAttribute(string contentType)
    {
        ContentType = contentType;
    }

    public string ContentType { get; }
}

/// <summary>Route requests with HTTP Method of <see cref="HttpMethod"/> with Route Pattern of <see cref="ApiPath"/> to the method.</summary>
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

/// <summary>Alias of <see cref="ApiRouteAttribute"/> for the GET HTTP Method.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiGetAttribute : ApiRouteAttribute
{
    public ApiGetAttribute([RouteTemplate] string apiPath) : base(HttpMethod.Get, apiPath)
    {
    }
}

/// <summary>Alias of <see cref="ApiRouteAttribute"/> for the POST HTTP Method.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiPostAttribute : ApiRouteAttribute
{
    public ApiPostAttribute([RouteTemplate] string apiPath) : base(HttpMethod.Post, apiPath)
    {
    }
}

/// <summary>Alias of <see cref="ApiRouteAttribute"/> for the DELETE HTTP Method.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiDeleteAttribute : ApiRouteAttribute
{
    public ApiDeleteAttribute([RouteTemplate] string apiPath) : base(HttpMethod.Delete, apiPath)
    {
    }
}

/// <summary>Alias of <see cref="ApiRouteAttribute"/> for the PUT HTTP Method.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiPutAttribute : ApiRouteAttribute
{
    public ApiPutAttribute([RouteTemplate] string apiPath) : base(HttpMethod.Put, apiPath)
    {
    }
}

/// <summary>Prefix the path of all <see cref="ApiRouteAttribute"/> with <see cref="ApiRootPath"/>.</summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public sealed class ApiRootAttribute : Attribute
{
    public ApiRootAttribute(string apiRootPath)
    {
        ApiRootPath = apiRootPath;
    }

    public string ApiRootPath { get; }
}