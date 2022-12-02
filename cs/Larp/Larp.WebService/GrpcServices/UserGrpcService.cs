using System.Security.Authentication;
using Grpc.Core;
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
    private readonly IEventService _eventService;
    private readonly IUserSessionManager _userSessionManager;

    public UserGrpcService(IUserSessionManager userSessionManager, IEventService eventService, ISystemClock clock)
    {
        _userSessionManager = userSessionManager;
        _eventService = eventService;
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

    public override async Task<AccountResponse> GetAccount(Empty request, ServerCallContext context)
    {
        var account = await _userSessionManager.GetUserAccount(context.GetAccountId())
                      ?? throw new ResponseException("Account not found");
        return new AccountResponse() { Account = account.ToProto() };
    }

    public override async Task<EventListResponse> GetEvents(EventListRequest request, ServerCallContext context)
    {
        var events = await _eventService.ListEventsForAccount(
            context.GetAccountId(),
            request.IncludePast,
            request.IncludeFuture,
            request.IncludeAttendance);

        var response = new EventListResponse();
        response.Event.AddRange(events);
        return response;
    }

    public override async Task<Event> RsvpEvent(EventRsvpRequest request, ServerCallContext context)
    {
        var ev = await _eventService.Rsvp(context.GetAccountId(), request.EventId, request.Rsvp);
        return ev;
    }

    public override async Task<Event> GetEvent(EventRequest request, ServerCallContext context)
    {
        return await _eventService.GetEvent(request.EventId);
    }
}

public class AdminGrpcService : LarpAdmin.LarpAdminBase
{
    public override Task<AccountResponse> SetAccount(AccountRequest request, ServerCallContext context)
    {
        throw new NotImplementedException("TBD");
    }
}