using JetBrains.Annotations;

namespace Larp.Landing.Shared;

[AttributeUsage(AttributeTargets.Method)]
public class ApiAuthenticatedAttribute : Attribute
{
}

public abstract class ApiRouteAttribute : Attribute
{
    protected ApiRouteAttribute(HttpMethod httpMethod, string apiPath)
    {
        HttpMethod = httpMethod;
        ApiPath = apiPath;
    }

    public HttpMethod HttpMethod { get; }
    public string ApiPath { get; }
}

[AttributeUsage(AttributeTargets.Method)]
public class ApiGetAttribute : ApiRouteAttribute
{
    public ApiGetAttribute([RouteTemplate]string apiPath) : base(HttpMethod.Get, apiPath)
    { }
}

[AttributeUsage(AttributeTargets.Method)]
public class ApiPost : ApiRouteAttribute
{
    public ApiPost([RouteTemplate]string apiPath) : base(HttpMethod.Post, apiPath)
    { }
}


[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class ApiRootAttribute : Attribute
{
    public string ApiRootPath { get; }
    
    public ApiRootAttribute(string apiRootPath)
    {
        ApiRootPath = apiRootPath;
    }
}