using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using Larp.Notify;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Larp.Landing.Server.Services;

public class LandingServiceServer : ILandingService
{
    private readonly LarpContext _db;
    private readonly IUserSessionManager _userSessionManager;
    private readonly IUserSession _userSession;
    private readonly INotifyService _notifyService;
    private readonly LetterManager _letterManager;
    private readonly Account? _account;

    public LandingServiceServer(LarpContext db, IUserSessionManager userSessionManager, IUserSession userSession,
        INotifyService notifyService, LetterManager letterManager)
    {
        _db = db;
        _userSessionManager = userSessionManager;
        _userSession = userSession;
        _notifyService = notifyService;
        _letterManager = letterManager;
        _account = userSession.CurrentUser;
    }

    public async Task<Result> Login(string email, string deviceName)
    {
        var token = await _userSessionManager.GenerateToken(email, deviceName);

        await _notifyService.SendEmailAsync(email, "LARP Landing", @$"Your sign in code for LARP Landing is {token}");

        return Result.Success;
    }

    public async Task<Result> Logout()
    {
        if (_userSession.SessionId == null)
            return Result.Failed("You are not logged in");
        await _userSessionManager.DestroyUserSession(_userSession.SessionId);
        return Result.Success;
    }

    public async Task<StringResult> Confirm(string email, string token, string deviceName)
    {
        var sessionId = await _userSessionManager.CreateUserSession(email, token, deviceName);

        var result = await _userSessionManager.ValidateUserSession(sessionId);
        if (result.Account?.IsSuperAdmin == true)
        {
            if (!result.Account.Roles.Contains(AccountRole.AdminAccess))
                await _userSessionManager.UpdateUserAccount(result.Account.AccountId,
                    x =>
                        x.Set(y => y.Roles,
                            new[]
                            {
                                AccountRole.AccountAdmin,
                                AccountRole.AdminAccess,
                                AccountRole.MwFifthGameMaster
                            }));
        }

        return StringResult.Success(sessionId);
    }

    public async Task<Result> Validate()
    {
        var result = await _userSessionManager.ValidateUserSession(_userSession.SessionId);
        switch (result.StatusCode)
        {
            case UserSessionValidationResultStatusCode.Authenticated:
                return Result.Success;
            case UserSessionValidationResultStatusCode.Invalid:
            case UserSessionValidationResultStatusCode.NotConfirmed:
            case UserSessionValidationResultStatusCode.Expired:
            default:
                return Result.Failed(result.StatusCode.ToString());
        }
    }

    public async Task<Game[]> GetGames()
    {
        var games = await _db.Games.Find(x => true).ToListAsync();
        return games.ToArray();
    }

    public async Task<CharacterSummary[]> GetCharacters()
    {
        var gameState = await _db.MwFifthGame.GetGameState();
        var characters = await _db.MwFifthGame.CharacterRevisions
            .Find(x => x.State != CharacterState.Archived && x.AccountId == _userSession.AccountId!)
            .ToListAsync();
        return characters.Select(x => x.ToSummary(gameState)).ToArray();
    }

    public Task<Account> GetAccount()
    {
        return Task.FromResult(_userSession.CurrentUser!);
    }

    public async Task AccountEmailAdd(string email) =>
        await _userSessionManager.AddEmailAddress(_userSession.AccountId!, email);

    public async Task AccountEmailRemove(string email) =>
        await _userSessionManager.RemoveEmailAddress(_userSession.AccountId!, email);

    public async Task AccountEmailPreferred(string email) =>
        await _userSessionManager.PreferEmailAddress(_userSession.AccountId!, email);

    public async Task AccountUpdate(string? fullName, string? location, string? phone, string? allergies,
        DateOnly? birthDate) =>
        await _userSessionManager.UpdateUserAccount(_userSession.AccountId!, builder => builder
            .Set(x => x.Name, fullName)
            .Set(x => x.Location, location)
            .Set(x => x.Phone, phone)
            .Set(x => x.Notes, allergies)
            .Set(x => x.BirthDate, birthDate)
        );

    public async Task<EventAndLetter[]> GetEvents(EventList list)
    {
        var now = DateOnly.FromDateTime(DateTime.Today);

        var letters = _userSession.CurrentUser == null
            ? new Dictionary<string, Letter>()
            : (await _db.Letters.Find(x => x.AccountId == _userSession.AccountId)
                .Project(x => new Letter() { LetterId = x.LetterId, EventId = x.EventId, State = x.State }).ToListAsync())
            .ToDictionary(x=>x.EventId);

        var filter = list switch
        {
            EventList.Past => Builders<Event>.Filter.Where(x => x.Date <= now.AddDays(4) && !x.IsHidden),
            EventList.Upcoming => Builders<Event>.Filter.Where(x => x.Date >= now.AddDays(-4) && !x.IsHidden),
            _ => throw new ArgumentOutOfRangeException(nameof(list), list, null)
        };

        var events = await _db.Events.Find(filter).ToListAsync();
        return events
            .Select(x=> new EventAndLetter()
            {
                Event = x,
                Letter = letters.GetValueOrDefault(x.EventId)
            }).ToArray();
    }

    public async Task<Dictionary<string, string>> GetCharacterNames()
    {
        var characters = await _db.MwFifthGame.Characters
            .Find(character => character.AccountId == _userSession.AccountId)
            .Project(character => new { UniqueId = character.CharacterId, character.CharacterName })
            .ToListAsync();
        return characters
            .ToDictionary(
                x => x.UniqueId,
                x => x.CharacterName ?? "No Name Set");
    }

    public async Task<EventAttendance[]> GetAttendance()
    {
        var letters = (await _db.Letters.AsQueryable()
                .Where(x => x.AccountId == _userSession.AccountId)
                .Select(x => new { x.EventId, x.State })
                .ToListAsync())
            .ToDictionary(x => x.EventId, x => x.State);

        var attendances =
            await _db.Attendances.AsQueryable()
                .Where(attendance => attendance.AccountId == _userSession.AccountId)
                .Join(
                    _db.Events.AsQueryable(),
                    attendance => attendance.EventId,
                    @event => @event.EventId,
                    (attendance, @event) => new { Attedance = attendance, Event = @event })
                .ToListAsync();

        return attendances
            .Select(x => new EventAttendance(
                x.Attedance,
                x.Event,
                letters.GetValueOrDefault(x.Event.EventId, LetterState.NotStarted)))
            .ToArray();
    }

    public async Task<Letter> DraftLetter(string eventId)
    {
        var templateId =
            await _db.Events.Find(x => x.EventId == eventId)
                .Project(x => x.LetterTemplateId)
                .FirstOrDefaultAsync()
            ?? throw new BadRequestException($"Event {eventId} does not have {nameof(Event.LetterTemplateId)}");
        return await _letterManager.Draft(templateId, eventId, _account!.AccountId);
    }

    public async Task<Letter[]> GetLetters() =>
        await _letterManager.GetAll(_account!.AccountId);

    public async Task<Letter> GetLetter(string letterId) =>
        await _letterManager.Get(letterId, _account!.AccountId, isAdmin: false);

    public async Task SaveLetter(string letterId, Letter letter)
    {
        letter.LetterId = letterId;
        await _letterManager.Save(letter, _account!.AccountId);
    }

    public async Task<LetterAndTemplate> GetEventLetter(string eventId) =>
        await _letterManager.GetEventLetter(_account!.AccountId, eventId);
}