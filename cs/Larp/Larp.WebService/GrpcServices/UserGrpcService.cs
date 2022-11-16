using System.Security.Authentication;
using Grpc.Core;
using Larp.Data;
using Larp.Data.Services;
using Larp.Protos;
using Larp.Protos.Services;
using Microsoft.Extensions.Internal;
using MongoDB.Driver;
using Account = Larp.Data.Account;

namespace Larp.WebService.GrpcServices;

public static class ServerCallContextExtensions
{
    public static Account GetAccount(this ServerCallContext context)
    {
        if (context.UserState["Account"] is not Account account)
            throw new AuthenticationException("Account is invalid");
        return account;
    }

    public static string GetAccountId(this ServerCallContext context)
    {
        if (context.UserState["Account"] is not Account account)
            throw new AuthenticationException("Account is invalid");
        return account.AccountId;
    }
}

public class ResponseException : Exception
{
    public ResponseException(string message) : base(message)
    {
    }

    public ResponseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class UserGrpcService : LarpUser.LarpUserBase
{
    private readonly ISystemClock _clock;
    private readonly LarpContext _larpContext;
    private readonly IUserSessionManager _userSessionManager;

    public UserGrpcService(LarpContext larpContext, IUserSessionManager userSessionManager, ISystemClock clock)
    {
        _larpContext = larpContext;
        _userSessionManager = userSessionManager;
        _clock = clock;
    }

    public override async Task<AccountResponse> AddEmail(StringRequest request, ServerCallContext context)
    {
        var accountId = context.GetAccountId();
        await _userSessionManager.AddEmailAddress(accountId, request.Value);
        return await GetAccountResponse(accountId);
    }

    public override async Task<AccountResponse> PreferEmail(StringRequest request, ServerCallContext context)
    {
        var accountId = context.GetAccountId();
        await _userSessionManager.PreferEmailAddress(accountId, request.Value);
        return await GetAccountResponse(accountId);
    }

    public override async Task<AccountResponse> RemoveEmail(StringRequest request, ServerCallContext context)
    {
        var accountId = context.GetAccountId();
        await _userSessionManager.RemoveEmailAddress(accountId, request.Value);
        return await GetAccountResponse(accountId);
    }

    private async Task<AccountResponse> GetAccountResponse(string accountId)
    {
        var account = await _userSessionManager.GetUserAccount(accountId);
        return new AccountResponse
        {
            Account = account.ToProto()
        };
    }

    public override async Task<AccountResponse> UpdateProfile(UpdateProfileRequest request, ServerCallContext context)
    {
        var accountId = context.GetAccountId();

        if (!request.HasName && !request.HasPhone && !request.HasLocation)
            return new AccountResponse() { Account = context.GetAccount().ToProto() };

        await _userSessionManager.UpdateUserAccount(accountId, builder =>
        {
            var update = builder.Set(x => x.LastUpdate, _clock.UtcNow);
            if (request.HasName)
                update = update.Set(x => x.Name, request.Name);
            if (request.HasPhone)
                update = update.Set(x => x.Phone, request.Phone);
            if (request.HasLocation)
                update = update.Set(x => x.Location, request.Location);
            if (request.HasNotes)
                update = update.Set(x => x.Notes, request.Notes);
            return update;
        });

        return new AccountResponse() { Account = context.GetAccount().ToProto() };
    }

    public override Task<AccountResponse> GetAccount(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new AccountResponse() { Account = context.GetAccount().ToProto() });
    }
}

public class AdminGrpcService : Larp.Protos.Services.LarpAdmin.LarpAdminBase
{
    public override Task<AccountResponse> SetAccount(AccountRequest request, ServerCallContext context)
    {
        return base.SetAccount(request, context);
    }
}