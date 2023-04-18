using System.Security.Claims;
using Larp.Landing.Server.Services;
using Microsoft.AspNetCore.Authentication;

namespace Larp.Landing.Server.Services;

public class LarpAuthenticationHandler : IAuthenticationHandler
{
    private readonly IUserSessionManager _userSessionManager;
    private readonly IUserSession _userSession;
    private string? _token;

    public LarpAuthenticationHandler(IUserSessionManager userSessionManager, IUserSession userSession)
    {
        _userSessionManager = userSessionManager;
        _userSession = userSession;
    }

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext httpContext)
    {
        var authHeader = httpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader)) return Task.CompletedTask;
        _token = authHeader.Split(' ').Last();
        return Task.CompletedTask;
    }

    public async Task<AuthenticateResult> AuthenticateAsync()
    {
        if (string.IsNullOrEmpty(_token))
            return AuthenticateResult.NoResult();
        
        var session = await _userSessionManager.ValidateUserSession(_token);
        var isAuthenticated = session.StatusCode == UserSessionValidationResultStatusCode.Authenticated;
        _userSession.Initialize(session);
        
        if (!isAuthenticated || session.Account == null)
            return AuthenticateResult.NoResult();

        var claims = new List<Claim>
        {
            new (ClaimTypes.Sid, session.Account.AccountId)
        };
        claims.AddRange(session.Account.Roles.Select(x => new Claim(ClaimTypes.Role, x.ToString())));
        
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer")), "Default"));
    }

    public Task ChallengeAsync(AuthenticationProperties? properties)
    {
        throw new NotImplementedException();
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        throw new NotImplementedException();
    }
}