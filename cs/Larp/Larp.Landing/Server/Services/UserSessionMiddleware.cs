namespace Larp.Landing.Server.Services;

public class UserSessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly UserManager? _userManager;

    public UserSessionMiddleware(RequestDelegate next, IUserManager userManager)
    {
        _next = next;
        _userManager = userManager as UserManager;
    }
    
    public async Task Invoke(HttpContext httpContext)
    {
        if (_userManager == null) return;
        await _userManager.GetCurrentUser(httpContext);
        await _next.Invoke(httpContext);
    }
}