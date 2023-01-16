using Larp.Data;
using Larp.Data.Services;

namespace Larp.WebService.Services;

public class UserSessionState
{
    private readonly IUserSessionManager _userSessionManager;

    public UserSessionState(IUserSessionManager userSessionManager)
    {
        _userSessionManager = userSessionManager;
    }

    public string SessionId { get; private set; } = "";
    public string AccountId { get; private set; } = null!;
    public Account Account { get; private set; } = null!;
    public bool IsAuthenticated { get; private set; }

    public async Task<bool> Authenticate(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId) || sessionId == "bypass")
            return false;

        var result = await _userSessionManager.ValidateUserSession(sessionId);

        if (result.StatusCode != UserSessionValidationResultStatusCode.Authenticated)
            return false;

        SessionId = sessionId;
        Account = result.Account!;
        AccountId = result.Account!.AccountId;
        IsAuthenticated = result.Account != null;
        return true;
    }
}