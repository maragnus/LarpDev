using System.Diagnostics.SymbolStore;
using System.Net;
using System.Security.Cryptography.Xml;
using Larp.Data;
using Larp.Data.Services;
using Larp.Proto;
using Microsoft.AspNetCore.Authentication;
using MongoDB.Driver;

namespace Larp.WebService.LarpServices;

public record AuthenticationResult(bool IsSuccess, string? Message, string? SessionId);

public interface IAuthenticationService
{
    Task<AuthenticationResult> InitiateLogin(string email, string deviceId);
    Task<AuthenticationResult> ConfirmLogin(string email, string code);
    Task<AuthenticationResult> ValidateSession(string sessionId);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly LarpContext _larpContext;
    private readonly IUserSessionService _sessions;
    private readonly IUserNotificationService _notificationService;
    private readonly ISystemClock _clock;

    public AuthenticationService(LarpContext larpContext, IUserSessionService sessions, IUserNotificationService notificationService, ISystemClock clock)
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
                .Project(x=>x.AccountId)
                .FirstOrDefaultAsync();
        
        if (accountId == null)
            return new(false, "Email address was not found", "");

        var token = await _sessions.GenerateToken(accountId, deviceId);

        await _notificationService.SendAuthenticationToken(accountId, token);
        
        return new(true, "Code is required to continue", "");
    }

    public async Task<AuthenticationResult> ConfirmLogin(string email, string code)
    {
        var filter = Builders<Data.Account>.Filter.ElemMatch(x => x.Emails, e => e.Email == email);
        var account =
            await _larpContext.Accounts
                .Find(filter)
                .FirstOrDefaultAsync();

        if (account == null)
            return new(false, "Account not found", null);

        try
        {
            var sessionId = await _sessions.CreateUserSession(code);
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
        return result switch
        {
            UserSessionValidationResult.Authenticated => new(true, null, sessionId),
            UserSessionValidationResult.Expired => new(false, "Session has expired", null),
            UserSessionValidationResult.Invalid => new(false, "Session could not be found", null),
            UserSessionValidationResult.NotConfirmed => new(false, "Session has not been confirmed", null),
            _ => throw new InvalidOperationException(
                $"{nameof(UserSessionValidationResult)} value of {result} has not been implemented")
        };
    }
}