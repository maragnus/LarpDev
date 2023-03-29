namespace Larp.Landing.Server.Services;

public class UserSessionMiddleware
{
    private readonly RequestDelegate _next;

    public UserSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext httpContext, IUserSession userSession)
    {
        await ((UserSession)userSession).GetCurrentUser(httpContext);
        await _next.Invoke(httpContext);
    }
}