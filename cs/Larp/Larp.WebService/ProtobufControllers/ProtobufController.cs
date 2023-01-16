using Microsoft.Extensions.Logging.Abstractions;

namespace Larp.WebService.ProtobufControllers;

public abstract class ProtobufController
{
    internal HttpContext HttpContextInternal = null!;
    internal ILogger LoggerInternal = NullLogger.Instance;

    /// <summary>Logger is populated immediately after constructor</summary>
    protected ILogger Logger => LoggerInternal;

    protected HttpContext HttpContext => HttpContextInternal;

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