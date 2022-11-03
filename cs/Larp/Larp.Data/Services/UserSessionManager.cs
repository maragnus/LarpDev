using System.Runtime.InteropServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Data.Services;

public enum UserSessionValidationResult
{
    Invalid = 0, // Cannot find user or session
    NotConfirmed = 1, // Created but not yet confirmed
    Authenticated = 2, // Fully authenticated
    Expired = 3, // Previously authenticated but now expired
}

public interface IUserSessionManager
{
    Task<string> GenerateToken(string accountId, string email, string deviceId);
    Task<string> CreateUserSession(string accountId, string token);
    Task DestroyUserSessions(string accountId);
    Task DestroyUserSession(string sessionId);
    Task<UserSessionValidationResult> ValidateUserSession(string sessionId);
    Task ConfirmEmailAddress(string accountId, string email);

    Task<Account> GetUserAccount(string accountId);
    void UserAccountChanged(string accountId);
}

public class UserSessionManager : IUserSessionManager
{
    private readonly LarpContext _larpContext;
    private readonly LarpDataCache _cache;
    private readonly ISystemClock _clock;
    private readonly UserSessionManagerOptions _options;

    public UserSessionManager(LarpContext larpContext, LarpDataCache cache, ISystemClock clock, IOptions<UserSessionManagerOptions> options)
    {
        _larpContext = larpContext;
        _cache = cache;
        _clock = clock;
        _options = options.Value;
    }

    private static string RandomReadableString(int length)
    {
        const string chars = "23456789CEFHKLPTX"; // Most readable characters
        var token = new char[length];
        for (var i = 0; i < length; i++)
            token[i] = chars[Random.Shared.Next(0, chars.Length)];
        return new string(token);
    }

    private async Task<string> GetUnusedToken()
    {
        while (true)
        {
            var token = RandomReadableString(_options.TokenLength);
            // Make sure we always have a unique token
            var isUsed = await _larpContext.Sessions.Find(x => x.Token == token).AnyAsync();
            if (isUsed) continue;
            return token;
        }
    }

    private static readonly SemaphoreSlim TokenLock = new(1);
    
    public async Task<string> GenerateToken(string accountId, string email, string deviceId)
    {
        await TokenLock.WaitAsync();
        try
        {
            var token = await GetUnusedToken();

            var session = new Session()
            {
                Token = token,
                AccountId = accountId,
                Email = email,
                DeviceId = deviceId,
                CreatedOn = _clock.UtcNow,
                DestroyedOn = null,
                ExpiresOn = _clock.UtcNow + _options.UserSessionDuration,
                ActivatedOn = null,
                SessionId = ObjectId.GenerateNewId().ToString(),
                IsAuthenticated = false // Set to true after Token is received
            };

            await _larpContext.Sessions.InsertOneAsync(session);

            return token;
        }
        finally
        {
            TokenLock.Release();
        }
    }
    
    public async Task<string> CreateUserSession(string accountId, string token)
    {
        var session = 
            await _larpContext.Sessions.Find(x => x.Token == token && x.AccountId == accountId).FirstOrDefaultAsync()
            ?? throw new UserSessionException("Token was not found");

        await ConfirmEmailAddress(session.AccountId, session.Email!);

        var update = Builders<Session>.Update
            .Set(s => s.Token, null)
            .Set(s=>s.ActivatedOn, _clock.UtcNow)
            .Set(s=>s.IsAuthenticated, true)
            .Set(s =>s.Email, null);

        await _larpContext.Sessions.UpdateOneAsync(x => x.Token == token, update);
        
        return session.SessionId;
    }

    public async Task ConfirmEmailAddress(string accountId, string email)
    {
        var accountFilter = Builders<Account>.Filter.And(
            Builders<Account>.Filter.Eq(x=>x.AccountId, accountId),
            Builders<Account>.Filter.ElemMatch(x => x.Emails, x => x.Email == email));
        var accountUpdate = Builders<Account>.Update.Set(x=>x.Emails[-1].IsVerified, true);
        await _larpContext.Accounts.UpdateOneAsync(accountFilter, accountUpdate);
    }
    
    public async Task DestroyUserSessions(string accountId)
    {
        var update = Builders<Session>.Update
            .Set(s => s.Token, null)
            .Set(s=>s.DestroyedOn, _clock.UtcNow)
            .Set(s=>s.IsAuthenticated, false);
        await _larpContext.Sessions.UpdateManyAsync(x => x.AccountId == accountId, update);
    }

    public async Task DestroyUserSession(string sessionId)
    {
        var update = Builders<Session>.Update
            .Set(s => s.Token, null)
            .Set(s=>s.DestroyedOn, _clock.UtcNow)
            .Set(s=>s.IsAuthenticated, false);
        await _larpContext.Sessions.UpdateManyAsync(x => x.SessionId == sessionId, update);
    }

    public async Task<Account> GetUserAccount(string accountId)
    {
        if (!_cache.TryGetValue(accountId, out Account account))
        {
            account =
                await _larpContext.Accounts.Find(x => x.AccountId == accountId).FirstOrDefaultAsync();
            
            _cache.Set(accountId, account, _options.CacheDuration);
        }

        return account;
    }

    public void UserAccountChanged(string accountId)
    {
        _cache.Remove(accountId);
    }
    
    public async Task<UserSessionValidationResult> ValidateUserSession(string sessionId)
    {
        if (!_cache.TryGetValue(sessionId, out Session session))
        {
            session =
                await _larpContext.Sessions.Find(x => x.SessionId == sessionId).FirstOrDefaultAsync();
            
            _cache.Set(sessionId, session, _options.CacheDuration);
        }

        if (session == null)
            return UserSessionValidationResult.Invalid;

        if (session.ExpiresOn <= _clock.UtcNow)
            return UserSessionValidationResult.Expired;

        if (session.IsAuthenticated)
            return UserSessionValidationResult.Authenticated;

        return UserSessionValidationResult.NotConfirmed;
    }
}