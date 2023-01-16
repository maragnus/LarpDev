using Microsoft.Extensions.Logging.Abstractions;

namespace Larp.WebService.ProtobufControllers;

public class RequestHandler
{
    public RequestHandler(HttpContext httpContext)
    {
        HttpContext = httpContext;
    }

    public HttpContext HttpContext { get; }
    public bool IsRequestCompleted { get; set; }
}

public abstract class ProtobufController
{
    internal HttpContext HttpContextInternal = null!;
    internal ILogger LoggerInternal = NullLogger.Instance;

    /// <summary>Logger is populated immediately after constructor</summary>
    protected ILogger Logger => LoggerInternal;

    protected HttpContext HttpContext => HttpContextInternal;

    public virtual ValueTask BeforeRequest(RequestHandler request)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask AfterResponse(HttpContext httpContext)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask<bool> BeforeResponse(RequestHandler request)
    {
        return ValueTask.FromResult(false);
    }
}