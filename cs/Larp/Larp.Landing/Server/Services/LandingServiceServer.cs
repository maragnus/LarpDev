using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using Larp.Notify;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class LandingServiceServer : ILandingService
{
    private readonly LarpContext _db;
    private readonly IUserSessionManager _userSessionManager;
    private readonly IUserSession _userSession;
    private readonly INotifyService _notifyService;

    public LandingServiceServer(LarpContext db, IUserSessionManager userSessionManager, IUserSession userSession,
        INotifyService notifyService)
    {
        _db = db;
        _userSessionManager = userSessionManager;
        _userSession = userSession;
        _notifyService = notifyService;
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
        var characters = await _db.MwFifthGame.Characters
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
}