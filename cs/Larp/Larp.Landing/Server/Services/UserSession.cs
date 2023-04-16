namespace Larp.Landing.Server.Services;

public interface IUserSession
{
    Account? CurrentUser { get; }
    string? SessionId { get; }
    bool IsAuthenticated { get; }
    string? AccountId { get; }
    bool HasAnyRole(AccountRole[] anyOfRoles);
    bool HasRole(AccountRole role);
}

public class UserSession : IUserSession
{
    private readonly IUserSessionManager _userSessionManager;

    public UserSession(IUserSessionManager userSessionManager)
    {
        _userSessionManager = userSessionManager;
    }

    public async Task GetCurrentUser(HttpContext httpContext)
    {
        var authHeader = httpContext.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader))
        {
            var token = authHeader.Split(' ').Last();
            var session = await _userSessionManager.ValidateUserSession(token);
            IsAuthenticated = session.StatusCode == UserSessionValidationResultStatusCode.Authenticated;
            AccountId = session.Account?.AccountId;
            CurrentUser = session.Account;
            SessionId = token;
        }
    }

    public string? AccountId { get; private set; }

    public bool HasAnyRole(AccountRole[] anyOfRoles) =>
        anyOfRoles.Length == 0
        || (CurrentUser?.Roles.Intersect(anyOfRoles).Any() ?? false);

    public bool HasRole(AccountRole role) => 
        CurrentUser?.Roles.Contains(role) ?? false;

    public Account? CurrentUser { get; private set; }
    public string? SessionId { get; private set; }
    public bool IsAuthenticated { get; private set; }
}