namespace Larp.Landing.Server.Services;

public interface IUserSession
{
    Account? Account { get; }
    string? SessionId { get; }
    bool IsAuthenticated { get; }
    string? AccountId { get; }
    bool HasAnyRole(AccountRole[] anyOfRoles);
    bool HasRole(AccountRole role);
    void Initialize(UserSessionValidationResult session);
}

public class UserSession : IUserSession
{
    public string? AccountId { get; private set; }

    public bool HasAnyRole(AccountRole[] anyOfRoles) =>
        anyOfRoles.Length == 0
        || (Account?.Roles.Intersect(anyOfRoles).Any() ?? false);

    public bool HasRole(AccountRole role) => 
        Account?.Roles.Contains(role) ?? false;

    public void Initialize(UserSessionValidationResult session)
    {
        IsAuthenticated = session.StatusCode == UserSessionValidationResultStatusCode.Authenticated;
        AccountId = session.Account?.AccountId;
        Account = session.Account;
    }

    public Account? Account { get; private set; }
    public string? SessionId { get; private set; }
    public bool IsAuthenticated { get; private set; }
}