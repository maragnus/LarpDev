namespace Larp.Landing.Shared;

public class ApiRoute : Attribute
{
    public ApiRoute(HttpMethod httpMethod, string apiPath)
    {
        HttpMethod = httpMethod;
        ApiPath = apiPath;
    }

    public HttpMethod HttpMethod { get; }
    public string ApiPath { get; }
}

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class ApiGet : ApiRoute
{
    public ApiGet(string apiPath) : base(HttpMethod.Get, apiPath)
    { }
}

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class ApiPost : ApiRoute
{
    public ApiPost(string apiPath) : base(HttpMethod.Post, apiPath)
    { }
}


[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true)]
public class ApiRootAttribute : Attribute
{
    public string ApiRootPath { get; }
    
    public ApiRootAttribute(string apiRootPath)
    {
        ApiRootPath = apiRootPath;
    }
}