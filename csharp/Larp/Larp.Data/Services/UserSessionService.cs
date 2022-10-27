using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
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

public interface IUserSessionService
{
    Task<string> GenerateToken(string accountId, string deviceId);
    Task<string> CreateUserSession(string token);
    Task DestroyUserSessions(string accountId);
    Task DestroyUserSession(string sessionId);
    Task<UserSessionValidationResult> ValidateUserSession(string sessionId);
}

public class UserSessionService : IUserSessionService
{
    private readonly LarpContext _larpContext;
    private readonly LarpDataCache _cache;
    private readonly ISystemClock _clock;
    private readonly UserSessionServiceOptions _options;

    public UserSessionService(LarpContext larpContext, LarpDataCache cache, ISystemClock clock, IOptions<UserSessionServiceOptions> options)
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
    
    public async Task<string> GenerateToken(string accountId, string deviceId)
    {
        await TokenLock.WaitAsync();
        try
        {
            var token = await GetUnusedToken();

            var session = new Session()
            {
                Token = token,
                AccountId = accountId,
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
    
    public async Task<string> CreateUserSession(string token)
    {
        var session = 
            await _larpContext.Sessions.Find(x => x.Token == token).FirstOrDefaultAsync()
            ?? throw new UserSessionException("Token was not found");

        var update = Builders<Session>.Update
            .Set(s => s.Token, null)
            .Set(s=>s.ActivatedOn, _clock.UtcNow)
            .Set(s=>s.IsAuthenticated, true);

        await _larpContext.Sessions.UpdateOneAsync(x => x.Token == token, update);

        return session.SessionId;
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