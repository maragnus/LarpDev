using Larp.Common.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Larp.Data.Mongo.Services;

public enum UserSessionValidationResultStatusCode
{
    Invalid = 0, // Cannot find user or session
    NotConfirmed = 1, // Created but not yet confirmed
    Authenticated = 2, // Fully authenticated
    Expired = 3, // Previously authenticated but now expired
}

public record UserSessionValidationResult(UserSessionValidationResultStatusCode StatusCode, Account? Account = null);

public interface IUserSessionManager
{
    Task<string> GenerateToken(string email, string deviceId);
    Task<string> GenerateToken(string accountId, string email, string deviceId);
    Task<string> CreateUserSession(string email, string token, string deviceName);
    Task DestroyUserSessions(string accountId);
    Task DestroyUserSession(string sessionId);
    Task<UserSessionValidationResult> ValidateUserSession(string? sessionId);
    Task ConfirmEmailAddress(string accountId, string email);
    Task PreferEmailAddress(string accountId, string email);
    Task AddEmailAddress(string accountId, string email);
    Task RemoveEmailAddress(string accountId, string email);

    Task<Account> GetUserAccount(string accountId);
    Task UpdateUserAccount(string accountId, Func<UpdateDefinitionBuilder<Account>, UpdateDefinition<Account>> builder);
    void UserAccountChanged(string accountId);
    Task AddAccountRole(string accountId, AccountRole role);
    Task RemoveAccountRole(string accountId, AccountRole role);
    Task<Account?> FindByEmail(string email);
    Task<string> AddAdminAccount(string fullName, string emailAddress);
}

public class UserSessionManager : IUserSessionManager
{
    private static readonly SemaphoreSlim TokenLock = new(1);
    private readonly LarpDataCache _cache;
    private readonly ISystemClock _clock;
    private readonly ILogger<UserSessionManager> _logger;
    private readonly LarpContext _larpContext;
    private readonly UserSessionManagerOptions _options;

    public UserSessionManager(LarpContext larpContext, LarpDataCache cache, ISystemClock clock,
        IOptions<UserSessionManagerOptions> options, ILogger<UserSessionManager> logger)
    {
        _larpContext = larpContext;
        _cache = cache;
        _clock = clock;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<string> GenerateToken(string email, string deviceId)
    {
        var normalizedEmail = email.ToLowerInvariant();

        var accountId = await GetAccountIdFromEmail(email);
        if (accountId != null)
            return await GenerateToken(accountId, email, deviceId);

        _logger.LogInformation("Creating new account for {Email} on {DeviceId}", email, deviceId);

        var account = new Account
        {
            AccountId = ObjectId.GenerateNewId().ToString(),
            Created = DateTimeOffset.Now,
            Emails =
            {
                new AccountEmail()
                {
                    Email = email,
                    NormalizedEmail = normalizedEmail,
                    IsPreferred = true,
                    IsVerified = false
                }
            }
        };
        await _larpContext.Accounts.InsertOneAsync(account);

        return await GenerateToken(account.AccountId, email, deviceId);
    }

    public async Task<string> GenerateToken(string accountId, string email, string deviceId)
    {
        var normalizedEmail = email.ToLowerInvariant();

        await TokenLock.WaitAsync();
        try
        {
            var token = await GetUnusedToken();

            var session = new Session()
            {
                Token = token,
                AccountId = accountId,
                Email = normalizedEmail,
                DeviceId = deviceId,
                CreatedOn = _clock.UtcNow,
                DestroyedOn = null,
                ExpiresOn = _clock.UtcNow + _options.UserSessionDuration,
                ActivatedOn = null,
                SessionId = ObjectId.GenerateNewId().ToString(),
                IsAuthenticated = false // Set to true after Token is received
            };

            _logger.LogInformation("Generating token {Token} for {Email} on {DeviceId}", token, email, deviceId);

            await _larpContext.Sessions.InsertOneAsync(session);

            return token;
        }
        finally
        {
            TokenLock.Release();
        }
    }

    public async Task<string> CreateUserSession(string email, string token, string deviceName)
    {
        var accountId = await GetAccountIdFromEmail(email);

        var session =
            await _larpContext.Sessions.Find(x => x.Token == token.ToUpperInvariant() && x.AccountId == accountId).FirstOrDefaultAsync()
            ?? throw new UserSessionException("Token was not found");

        _logger.LogInformation("User {Email} has signed in on {DeviceId}", email, deviceName);

        await ConfirmEmailAddress(session.AccountId, session.Email!);

        var update = Builders<Session>.Update
            .Set(s => s.Token, null)
            .Set(s => s.ActivatedOn, _clock.UtcNow)
            .Set(s => s.IsAuthenticated, true)
            .Set(s => s.Email, null)
            .Set(s => s.DeviceId, deviceName);

        await _larpContext.Sessions.UpdateOneAsync(x => x.Token == token, update);

        // Update first login (if necessary)
        var firstLogin = await _larpContext.Accounts.Find(x => x.AccountId == accountId)
            .Project(x => x.FirstLogin)
            .FirstOrDefaultAsync();
        if (firstLogin == null)
        {
            await _larpContext.Accounts.UpdateOneAsync(
                x => x.AccountId == accountId,
                Builders<Account>.Update.Set(x => x.FirstLogin, DateTimeOffset.Now));
        }

        return session.SessionId;
    }

    private async Task<string?> GetAccountIdFromEmail(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var filter = Builders<Account>.Filter
            .ElemMatch(x => x.Emails, x => x.NormalizedEmail == normalizedEmail);

        var project = Builders<Account>.Projection
            .Expression(a => a.AccountId);

        return await _larpContext.Accounts
            .Find(filter)
            .Project(project)
            .FirstOrDefaultAsync();
    }

    public async Task ConfirmEmailAddress(string accountId, string email)
    {
        var normalizedEmail = email.ToLowerInvariant();

        var accountFilter = Builders<Account>.Filter.And(
            Builders<Account>.Filter.Eq(x => x.AccountId, accountId),
            Builders<Account>.Filter.ElemMatch(x => x.Emails, x => x.NormalizedEmail == normalizedEmail));
        var accountUpdate = Builders<Account>.Update
            .Set(x => x.Emails.FirstMatchingElement().IsVerified, true);
        await _larpContext.Accounts.UpdateOneAsync(accountFilter, accountUpdate);
        UserAccountChanged(accountId);
    }

    public async Task PreferEmailAddress(string accountId, string email)
    {
        var normalizedEmail = email.ToLowerInvariant();

        var accountFilter = Builders<Account>.Filter;
        var accountEmailFilter = Builders<AccountEmail>.Filter;

        // Unset previously preferred email
        {
            var filter = accountFilter.Eq(x => x.AccountId, accountId)
                         & accountFilter.ElemMatch(x => x.Emails,
                             x => x.NormalizedEmail != normalizedEmail && x.IsPreferred);
            var update = Builders<Account>.Update
                .Set(x => x.Emails.FirstMatchingElement().IsPreferred, false);
            await _larpContext.Accounts.UpdateOneAsync(filter, update);
        }

        // Set newly preferred email
        {
            var filter = accountFilter.Eq(x => x.AccountId, accountId)
                         & accountFilter.ElemMatch(x => x.Emails, x => x.NormalizedEmail == normalizedEmail);
            var update = Builders<Account>.Update.Set(x => x.Emails.FirstMatchingElement().IsPreferred, true);
            await _larpContext.Accounts.UpdateOneAsync(filter, update);
        }

        UserAccountChanged(accountId);
    }

    public async Task AddEmailAddress(string accountId, string email)
    {
        try
        {
            var accountFilter = Builders<Account>.Filter;
            var emailFilter = Builders<AccountEmail>.Filter;

            var normalizedEmail = email.ToLowerInvariant();

            // Make sure the email address doesn't already exist
            var emailExists = await _larpContext.Accounts
                .Find(accountFilter.ElemMatch(x => x.Emails, emailFilter.Eq(x => x.NormalizedEmail, normalizedEmail)))
                .FirstOrDefaultAsync();
            if (emailExists != null)
            {
                if (emailExists.AccountId == accountId)
                    return;
                throw new Exception("Email address has already been added to another account");
            }

            // Add the email address
            var filter = accountFilter.Eq(x => x.AccountId, accountId);
            var update = Builders<Account>.Update
                .AddToSet(x => x.Emails,
                    new AccountEmail()
                    {
                        Email = email,
                        NormalizedEmail = normalizedEmail,
                        IsPreferred = false,
                        IsVerified = false
                    });
            await _larpContext.Accounts.UpdateOneAsync(filter, update);

            UserAccountChanged(accountId);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            // Skip this transaction
        }
    }

    public async Task RemoveEmailAddress(string accountId, string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var accountFilter = Builders<Account>.Filter;
        var emailFilter = Builders<AccountEmail>.Filter;

        // Make sure the email address doesn't already exist
        var otherEmailExists = await _larpContext.Accounts
            .Find(accountFilter.ElemMatch(x => x.Emails, emailFilter.Ne(x => x.NormalizedEmail, normalizedEmail)))
            .AnyAsync();

        if (!otherEmailExists)
            throw new Exception("Cannot remove last email account");

        var accountUpdate = Builders<Account>.Update
            .PullFilter(x => x.Emails, x => x.NormalizedEmail == normalizedEmail);
        await _larpContext.Accounts.UpdateOneAsync(x => x.AccountId == accountId, accountUpdate);

        UserAccountChanged(accountId);
    }

    public async Task DestroyUserSessions(string accountId)
    {
        var update = Builders<Session>.Update
            .Set(s => s.Token, null)
            .Set(s => s.DestroyedOn, _clock.UtcNow)
            .Set(s => s.IsAuthenticated, false);
        await _larpContext.Sessions.UpdateManyAsync(x => x.AccountId == accountId, update);
    }

    public async Task DestroyUserSession(string sessionId)
    {
        var update = Builders<Session>.Update
            .Set(s => s.Token, null)
            .Set(s => s.DestroyedOn, _clock.UtcNow)
            .Set(s => s.IsAuthenticated, false);
        await _larpContext.Sessions.UpdateManyAsync(x => x.SessionId == sessionId, update);
    }

    public async Task<Account> GetUserAccount(string accountId)
    {
        if (_cache.TryGetValue(accountId, out Account? account))
            return account!;

        account = await _larpContext.Accounts.FindOneAsync(x => x.AccountId == accountId)
                  ?? throw new Exception("Account not found");

        _cache.Set(accountId, account, _options.CacheDuration);

        return account;
    }

    public async Task UpdateUserAccount(string accountId,
        Func<UpdateDefinitionBuilder<Account>, UpdateDefinition<Account>> builder)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new Exception("AccountId is required");

        var updateDefinition = builder(new UpdateDefinitionBuilder<Account>());

        await _larpContext.Accounts.UpdateOneAsync(x => x.AccountId == accountId, updateDefinition);
        UserAccountChanged(accountId);
    }

    public void UserAccountChanged(string accountId)
    {
        _cache.Remove(accountId);
    }

    public async Task AddAccountRole(string accountId, AccountRole role)
    {
        var account = await GetUserAccount(accountId)
                      ?? throw new UserSessionException("Account not found");

        var roles = account.Roles.ToHashSet();
        if (!roles.Add(role)) return;

        await UpdateUserAccount(accountId, x =>
            x.Set(a => a.Roles, roles.ToArray()));
    }

    public async Task RemoveAccountRole(string accountId, AccountRole role)
    {
        var account = await GetUserAccount(accountId)
                      ?? throw new UserSessionException("Account not found");

        var roles = account.Roles.ToHashSet();
        if (!roles.Remove(role)) return;

        await UpdateUserAccount(accountId, x =>
            x.Set(a => a.Roles, roles.ToArray()));
    }

    public async Task<Account> FindByEmail(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var filter = Builders<Account>.Filter
            .ElemMatch(x => x.Emails, x => x.NormalizedEmail == normalizedEmail);

        return await _larpContext.Accounts
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task<string> AddAdminAccount(string fullName, string emailAddress)
    {
        var result = await FindByEmail(emailAddress);
        if (result != null)
             throw new BadRequestException($"User {result.Name} exists with email {emailAddress}");

        var account = new Account()
        {
            AccountId = ObjectId.GenerateNewId().ToString(),
            Name = fullName,
            Emails =
            {
                new AccountEmail()
                {
                    Email = emailAddress,
                    NormalizedEmail = emailAddress.ToLowerInvariant(),
                    IsPreferred = true
                }
            },
            Roles = new[] { AccountRole.AdminAccess },
            Created = DateTimeOffset.Now
        };
        await _larpContext.Accounts.InsertOneAsync(account);
        return account.AccountId;
    }

    public async Task<UserSessionValidationResult> ValidateUserSession(string? sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return new UserSessionValidationResult(UserSessionValidationResultStatusCode.Invalid);

        if (!_cache.TryGetValue(sessionId, out Session? session))
        {
            session =
                await _larpContext.Sessions.Find(x => x.SessionId == sessionId).FirstOrDefaultAsync();

            _cache.Set(sessionId, session, _options.CacheDuration);
        }

        if (session == null)
            return new UserSessionValidationResult(UserSessionValidationResultStatusCode.Invalid);

        if (session.ExpiresOn <= _clock.UtcNow)
            return new UserSessionValidationResult(UserSessionValidationResultStatusCode.Expired);

        if (session.IsAuthenticated)
        {
            var account = await GetUserAccount(session.AccountId);
            return new UserSessionValidationResult(UserSessionValidationResultStatusCode.Authenticated, account);
        }

        return new UserSessionValidationResult(UserSessionValidationResultStatusCode.NotConfirmed);
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
}