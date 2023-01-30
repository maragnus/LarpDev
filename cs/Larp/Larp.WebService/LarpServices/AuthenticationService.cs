using Larp.Data;
using Larp.Data.Services;
using Microsoft.Extensions.Internal;
using MongoDB.Driver;

namespace Larp.WebService.LarpServices;

public record AuthenticationResult(bool IsSuccess, string? Message, string? SessionId, bool IsExpired = false);

public interface IAuthenticationService
{
    Task<AuthenticationResult> InitiateLogin(string email, string deviceId);
    Task<AuthenticationResult> ConfirmLogin(string email, string code);
    Task<AuthenticationResult> ValidateSession(string sessionId);
    Task DestroySession(string sessionId);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ISystemClock _clock;
    private readonly LarpContext _larpContext;
    private readonly IUserNotificationService _notificationService;
    private readonly IUserSessionManager _sessions;

    public AuthenticationService(LarpContext larpContext, IUserSessionManager sessions,
        IUserNotificationService notificationService, ISystemClock clock)
    {
        _larpContext = larpContext;
        _sessions = sessions;
        _notificationService = notificationService;
        _clock = clock;
    }

    public async Task<AuthenticationResult> InitiateLogin(string email, string deviceId)
    {
        var filter = Builders<Data.Account>.Filter.ElemMatch(x => x.Emails, e => e.Email == email);
        var accountId =
            await _larpContext.Accounts.Find(filter)
                .Project(x => x.AccountId)
                .FirstOrDefaultAsync();

        if (accountId == null)
        {
            var account = new Data.Account()
            {
                Created = _clock.UtcNow,
                Emails =
                {
                    new Data.AccountEmail()
                    {
                        Email = email, NormalizedEmail = email.ToLowerInvariant(), IsPreferred = true, IsVerified = true
                    }
                }
            };
            await _larpContext.Accounts.InsertOneAsync(account, new InsertOneOptions(), CancellationToken.None);
            accountId = account.AccountId;
        }

        var token = await _sessions.GenerateToken(accountId, email, deviceId);

        await _notificationService.SendAuthenticationToken(accountId, email, token);

        return new(true, "Code is required to continue", "");
    }

    public async Task<AuthenticationResult> ConfirmLogin(string email, string code)
    {
        var filter = Builders<Account>.Filter.ElemMatch(x => x.Emails, e => e.Email == email);
        var account =
            await _larpContext.Accounts
                .Find(filter)
                .FirstOrDefaultAsync();

        if (account == null)
            return new(false, "Account not found", null);

        try
        {
            var sessionId = await _sessions.CreateUserSession(account.AccountId, code);
            return new(true, null, sessionId);
        }
        catch (UserSessionException ex)
        {
            return new(false, ex.Message, null);
        }
    }

    public async Task<AuthenticationResult> ValidateSession(string sessionId)
    {
        var result = await _sessions.ValidateUserSession(sessionId);
        return result.StatusCode switch
        {
            UserSessionValidationResultStatusCode.Authenticated => new(true, null, sessionId),
            UserSessionValidationResultStatusCode.Expired => new(false, "Session has expired", null, true),
            UserSessionValidationResultStatusCode.Invalid => new(false, "Session could not be found", null),
            UserSessionValidationResultStatusCode.NotConfirmed => new(false, "Session has not been confirmed", null),
            _ => throw new InvalidOperationException(
                $"{nameof(UserSessionValidationResult)} value of {result} has not been implemented")
        };
    }

    public async Task DestroySession(string sessionId)
    {
        await _sessions.DestroyUserSession(sessionId);
    }
}