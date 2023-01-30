using Larp.Data.Services;
using Larp.Notify;

namespace Larp.WebService.LarpServices;

public interface IUserNotificationService
{
    public Task SendAuthenticationToken(string accountId, string email, string token);
}

public class UserNotificationService : IUserNotificationService
{
    private readonly ILogger<UserNotificationService> _logger;
    private readonly INotifyService _notifyService;
    private readonly IUserSessionManager _userSessionManager;

    public UserNotificationService(IUserSessionManager userSessionManager, INotifyService notifyService,
        ILogger<UserNotificationService> logger)
    {
        _userSessionManager = userSessionManager;
        _notifyService = notifyService;
        _logger = logger;
    }

    public async Task SendAuthenticationToken(string accountId, string email, string token)
    {
        _logger.LogInformation("Account {AccountId} authentication token is {Token}", accountId, token);

        // Convert login email into email from account (matching case)
        var account = await _userSessionManager.GetUserAccount(accountId);
        var emailAddress = account.Emails.Single(x => x.NormalizedEmail == email.ToLowerInvariant()).Email;

        await _notifyService.SendEmailAsync(emailAddress, "Your Login Code",
            $"Your verification code is {token}");
    }
}