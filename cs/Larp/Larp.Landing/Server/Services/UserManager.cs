using Larp.Data;
using Larp.Data.Mongo.Services;

namespace Larp.Landing.Server.Services;

public interface IUserManager
{
    Account? CurrentUser { get; }
}

public class UserManager : IUserManager
{
    private readonly IUserSessionManager _userSessionManager;

    private const string SessionCookieName = "UserToken"; 
    
    public UserManager(IUserSessionManager userSessionManager)
    {
        _userSessionManager = userSessionManager;
    }

    public async Task GetCurrentUser(HttpContext httpContext)
    {
        if (httpContext.Request.Cookies.TryGetValue(SessionCookieName, out var token))
        {
            var session = await _userSessionManager.ValidateUserSession(token);
            CurrentUser = session.Account;
        }
    }
    
    public async Task<Account?> Authenticate(HttpContext httpContext, string email)
    {
        if (httpContext.Request.Cookies.TryGetValue(SessionCookieName, out var token))
        {
            var session = await _userSessionManager.ValidateUserSession(token);
            return session.Account;
        }
        return null;
    }

    public Account? CurrentUser { get; private set; }
}