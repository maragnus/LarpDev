namespace Larp.WebService.Controllers;

public class SessionContext
{
    public SessionContext(string? sessionId, string? accountId, Data.Account? account)
    {
        SessionId = sessionId ?? "";
        AccountId = accountId ?? "";
        Account = account;
    }

    public string SessionId { get; }
    public string AccountId { get; }
    public Data.Account? Account { get; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(SessionId);
    public static SessionContext NoSession { get; } = new(null, null, null);
}