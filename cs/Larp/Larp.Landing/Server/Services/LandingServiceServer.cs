using Larp.Notify;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class LandingServiceServer : ILandingService
{
    private readonly LarpContext _db;
    private readonly IUserSessionManager _userSessionManager;
    private readonly IUserSession _userSession;
    private readonly INotifyService _notifyService;
    private readonly LetterManager _letterManager;
    private readonly EventManager _eventManager;
    private readonly string _accountId;

    public LandingServiceServer(LarpContext db, IUserSessionManager userSessionManager, IUserSession userSession,
        INotifyService notifyService, LetterManager letterManager, EventManager eventManager)
    {
        _db = db;
        _userSessionManager = userSessionManager;
        _userSession = userSession;
        _notifyService = notifyService;
        _letterManager = letterManager;
        _eventManager = eventManager;
        _accountId = userSession.AccountId!;
    }

    public async Task<Result> Login(string email, string deviceName)
    {
        var token = await _userSessionManager.GenerateToken(email, deviceName);
        await _notifyService.SendEmailAsync(email, "Mystwood Tavern", @$"Your sign in code for Mystwood Tavern is {token}");
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

        // Hack to easily give a user all admin roles
        if (result.Account?.IsSuperAdmin == true && !result.Account.Roles.Contains(AccountRole.AdminAccess))
            await GrantAdminRoles(result.Account.AccountId);

        return StringResult.Success(sessionId);
    }

    private async Task GrantAdminRoles(string accountId)
    {
        await _userSessionManager.UpdateUserAccount(accountId,
            x =>
                x.Set(y => y.Roles,
                    new[]
                    {
                        AccountRole.AccountAdmin,
                        AccountRole.AdminAccess,
                        AccountRole.MwFifthGameMaster
                    }));
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

    public async Task<Game[]> GetGames() =>
        await _db.Games.Find(x => true).ToArrayAsync();

    public async Task<CharacterSummary[]> GetCharacters()
    {
        var gameState = await _db.MwFifthGame.GetGameState();
        var characters = await _db.MwFifthGame.CharacterRevisions
            .Find(x => x.State != CharacterState.Archived && x.AccountId == _userSession.AccountId!)
            .ToListAsync();
        return characters.Select(x => x.ToSummary(gameState)).ToArray();
    }

    public Task<Account> GetAccount() =>
        Task.FromResult(_userSession.Account!);

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

    public async Task<EventsAndLetters> GetEvents(EventList list) =>
        await _eventManager.GetEvents(list, _userSession.AccountId);

    public async Task<Dictionary<string, string>> GetCharacterNames()
    {
        var characters = await _db.MwFifthGame.CharacterRevisions
            .Find(character => character.AccountId == _userSession.AccountId && character.State == CharacterState.Live)
            .Project(character => new { character.CharacterId, character.CharacterName })
            .ToListAsync();
        return characters
            .ToDictionary(
                x => x.CharacterId,
                x => x.CharacterName ?? "No Name Set");
    }

    public async Task<EventAttendance[]> GetAttendance() =>
        await _eventManager.GetAccountAttendances(_accountId);

    public async Task<Letter> DraftLetter(string eventId, string letterName) =>
        await _letterManager.Draft(_accountId, eventId, letterName);

    public async Task<Letter[]> GetLetters() =>
        await _letterManager.GetAll(_accountId);

    public async Task<Letter> GetLetter(string letterId) =>
        await _letterManager.Get(letterId, _accountId, isAdmin: false);

    public async Task SaveLetter(string letterId, Letter letter)
    {
        letter.LetterId = letterId;
        await _letterManager.Save(letter, _accountId);
    }

    public async Task<EventsAndLetters> GetEventLetter(string eventId, string letterName) =>
        await _letterManager.GetEventLetter(_accountId, eventId, letterName);

    public async Task<IFileInfo> GetAttachment(string attachmentId, string fileName)
    {
        var file =
            await _db.AccountAttachments.Find(x => x.AttachmentId == attachmentId)
                .Project(x=> new AccountAttachment()
                {
                    Data = x.Data,
                    FileName = x.FileName,
                    MediaType = x.MediaType
                })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException("Attachment not found");

        if (file.Data == null)
            throw new ResourceNotFoundException("Attachment data is invalid");

        return new DownloadFileInfo(new MemoryStream(file.Data), file.FileName ?? "file", file.Data.Length);
    }
    
    public async Task<IFileInfo> GetAttachmentThumbnail(string attachmentId, string fileName)
    {
        var file =
            await _db.AccountAttachments.Find(x => x.AttachmentId == attachmentId)
                .Project(x=> new AccountAttachment()
                {
                    ThumbnailData = x.ThumbnailData,
                    ThumbnailFileName = x.ThumbnailFileName,
                    ThumbnailMediaType = x.ThumbnailMediaType
                })
                .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException("Attachment not found");

        if (file.ThumbnailData == null)
            throw new ResourceNotFoundException("Attachment thumbnail data is invalid");

        return new DownloadFileInfo(new MemoryStream(file.ThumbnailData), file.ThumbnailFileName ?? "file", file.ThumbnailData.Length);
    }
}

public class DownloadFileInfo : IFileInfo
{
    private readonly MemoryStream _stream;

    public DownloadFileInfo(MemoryStream stream, string name, int length)
    {
        Name = name;
        Length = length;
        _stream = stream;
    }

    public Stream CreateReadStream() => _stream;

    public bool Exists => true;
    public long Length { get; }
    public string? PhysicalPath => null;
    public string Name { get; }
    public DateTimeOffset LastModified => DateTimeOffset.Now;
    public bool IsDirectory => false;
}