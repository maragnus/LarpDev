namespace Larp.WebService.LarpServices;

public interface IUserNotificationService
{
    public Task SendAuthenticationToken(string accountId, string token);
}

public class UserNotificationService : IUserNotificationService
{
    private readonly ILogger<UserNotificationService> _logger;

    public UserNotificationService(ILogger<UserNotificationService> logger)
    {
        _logger = logger;
    }
    
    public Task SendAuthenticationToken(string accountId, string token)
    {
        _logger.LogInformation("Account {AccountId} authentication token is {Token}", accountId, token);
        return Task.CompletedTask;
    }
}