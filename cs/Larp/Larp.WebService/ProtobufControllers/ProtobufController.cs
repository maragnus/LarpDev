namespace Larp.WebService.ProtobufControllers;

public abstract class ProtobufController
{
    public ValueTask BeforeRequest(HttpContext httpContext)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask AfterResponse(HttpContext httpContext)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> BeforeResponse(HttpContext httpContext)
    {
        return ValueTask.FromResult(false);
    }
}