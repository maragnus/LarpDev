namespace Larp.Landing.Shared;

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class ApiPathAttribute : Attribute
{
    public string ApiPath { get; }

    public ApiPathAttribute(string apiPath)
    {
        ApiPath = apiPath;
    }
}