using JetBrains.Annotations;
using Larp.Data.Services;
using Larp.Protos;
using Larp.Protos.Services;
using Larp.WebService.GrpcServices;
using Larp.WebService.ProtobufControllers;
using Microsoft.Extensions.Internal;
using MongoDB.Driver;

namespace Larp.WebService.Controllers;

public class SessionContext
{
    public string SessionId => "";
    public string GetAccountId() => SessionId;
    public Data.Account GetAccount() => null!;
}

public abstract class SessionProtobufController : ProtobufController
{
    protected SessionContext SessionContext { get; } = new SessionContext();
}

[PublicAPI]
public class UserController : SessionProtobufController
{
    private readonly ISystemClock _clock;
    private readonly IEventService _eventService;
    private readonly IUserSessionManager _userSessionManager;

    public UserController(IUserSessionManager userSessionManager, IEventService eventService, ISystemClock clock)
    {
        _userSessionManager = userSessionManager;
        _eventService = eventService;
        _clock = clock;
    }

    public async Task<AccountResponse> AddEmail(StringRequest request)
    {
        var accountId = SessionContext.GetAccountId();
        await _userSessionManager.AddEmailAddress(accountId, request.Value);
        return await GetAccountResponse(accountId);
    }

    public async Task<AccountResponse> PreferEmail(StringRequest request)
    {
        var accountId = SessionContext.GetAccountId();
        await _userSessionManager.PreferEmailAddress(accountId, request.Value);
        return await GetAccountResponse(accountId);
    }

    public async Task<AccountResponse> RemoveEmail(StringRequest request)
    {
        var accountId = SessionContext.GetAccountId();
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

    public async Task<AccountResponse> UpdateProfile(UpdateProfileRequest request)
    {
        var accountId = SessionContext.GetAccountId();

        if (!request.HasName && !request.HasPhone && !request.HasLocation)
            return new AccountResponse() { Account = SessionContext.GetAccount().ToProto() };

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

        return new AccountResponse() { Account = SessionContext.GetAccount().ToProto() };
    }

    public async Task<AccountResponse> GetAccount(Empty request)
    {
        var account = await _userSessionManager.GetUserAccount(SessionContext.GetAccountId())
                      ?? throw new ResponseException("Account not found");
        return new AccountResponse() { Account = account.ToProto() };
    }

    public async Task<EventListResponse> GetEvents(EventListRequest request)
    {
        var events = await _eventService.ListEventsForAccount(
            SessionContext.GetAccountId(),
            request.IncludePast,
            request.IncludeFuture,
            request.IncludeAttendance);

        var response = new EventListResponse();
        response.Event.AddRange(events);
        return response;
    }

    public async Task<Event> RsvpEvent(EventRsvpRequest request)
    {
        var ev = await _eventService.Rsvp(SessionContext.GetAccountId(), request.EventId, request.Rsvp);
        return ev;
    }

    public async Task<Event> GetEvent(EventRequest request)
    {
        return await _eventService.GetEvent(request.EventId);
    }
}