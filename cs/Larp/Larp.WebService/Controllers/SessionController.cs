using System.Net;
using Larp.WebService.ProtobufControllers;
using Larp.WebService.Services;

namespace Larp.WebService.Controllers;

public abstract class SessionController : ProtobufController
{
    protected SessionContext SessionContext { get; private set; } = SessionContext.NoSession;

    public override ValueTask BeforeRequest(RequestHandler request)
    {
        var state = request.HttpContext.RequestServices.GetRequiredService<UserSessionState>();
        if (state.IsAuthenticated)
        {
            SessionContext = new SessionContext(state.SessionId, state.AccountId, state.Account);
        }
        else
        {
            request.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            request.IsRequestCompleted = true;
        }

        return ValueTask.CompletedTask;
    }
}