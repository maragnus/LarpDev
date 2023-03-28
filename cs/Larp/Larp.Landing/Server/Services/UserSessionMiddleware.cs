namespace Larp.Landing.Server.Services;

public class UserSessionMiddleware
{
    private readonly RequestDelegate _next;

    public UserSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext httpContext, IUserManager userManager)
    {
        await ((UserManager)userManager).GetCurrentUser(httpContext);
        await _next.Invoke(httpContext);
    }
}