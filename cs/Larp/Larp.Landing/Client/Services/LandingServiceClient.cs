using Larp.Common;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Larp.Landing.Shared.MwFifth;
using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Client.Services;

public class LandingServiceClient : RestClient, ILandingService, IMwFifthService, IAdminService
{
    public LandingServiceClient(HttpClient httpClient, ILogger<LandingServiceClient> logger)
        : base(httpClient, logger)
    {
    }

    public async Task<Result> Login(string email, string deviceName) =>
        await Post<Result>("api/auth/login", new { Email = email, DeviceName = deviceName });

    public async Task<StringResult> Confirm(string email, string token, string deviceName) =>
        await Post<StringResult>("api/auth/confirm",
            new { Email = email, Token = token, DeviceName = deviceName });

    public async Task<Result> Logout() =>
        await Post<Result>("api/auth/logout");

    public async Task<Result> Validate() =>
        await Post<Result>("api/auth/validate");

    public async Task<Game[]> GetGames() =>
        await GetArray<Game>("api/larp/games");

    public async Task<CharacterSummary[]> GetCharacters() =>
        await GetArray<CharacterSummary>("api/larp/characters");

    public async Task<StringResult> Import(Stream data) =>
        await PostFile<StringResult>("api/admin/data/import", data, "import.xlsx");

    public async Task<IFileInfo> Export() =>
        await Download("api/admin/data/export");

    public async Task<IFileInfo> ExportLetters(string eventId) =>
        await Download($"api/admin/letters/events/{eventId}/export");

    public async Task MergeAccounts(string fromAccountId, string toAccountId) =>
        await Post("api/admin/accounts/merge", new { fromAccountId, toAccountId });

    public Task AddAccountEmail(string accountId, string email) =>
        Post($"api/admin/accounts/{accountId}/emails?email={Uri.EscapeDataString(email)}");

    public Task RemoveAccountEmail(string accountId, string email) =>
        Delete($"api/admin/accounts/{accountId}/emails?email={Uri.EscapeDataString(email)}");

    public async Task<LetterTemplate> DraftLetterTemplate() =>
        await Post<LetterTemplate>("api/admin/letters/templates/new");

    public async Task SaveLetterTemplate(string templateId, LetterTemplate template) =>
        await Post($"api/admin/letters/templates/{templateId}", new { template });

    public async Task<LetterTemplate[]> GetLetterTemplates() =>
        await Get<LetterTemplate[]>("api/admin/letters/templates");

    public async Task<LetterTemplate[]> GetLetterTemplateNames() =>
        await Get<LetterTemplate[]>("api/admin/letters/templates/names");

    public Task<Account> GetAccount() =>
        Get<Account>("api/account");

    public Task AccountEmailAdd(string email) =>
        Post($"api/account/email?email={Uri.EscapeDataString(email)}");

    public Task AccountEmailRemove(string email) =>
        Delete($"api/account/email?email={Uri.EscapeDataString(email)}");

    public Task AccountEmailPreferred(string email) =>
        Post($"api/account/email/preferred?email={Uri.EscapeDataString(email)}");

    public Task AccountUpdate(string? fullName, string? location, string? phone, string? allergies,
        DateOnly? birthDate) =>
        Post("api/account", new { fullName, location, phone, allergies, birthDate });

    public Task MoveMwFifthCharacter(string characterId, string newAccountId) =>
        Post($"api/admin/mw5e/characters/{characterId}/move", new { newAccountId });

    public Task<EventsAndLetters> GetEvents(EventList list) =>
        Get<EventsAndLetters>($"api/events?list={list}");

    public Task<Dictionary<string, string>> GetCharacterNames() =>
        Get<Dictionary<string, string>>("api/larp/characters/names");

    public Task<EventAttendance[]> GetAttendance() =>
        Get<EventAttendance[]>("api/events/attendance");

    public Task<Letter> DraftLetter(string eventId, string letterName) =>
        Post<Letter>("api/letters/new", new { eventId, letterName });

    public Task<Letter[]> GetLetters() =>
        Get<Letter[]>("api/letters");

    public async Task ApproveLetter(string letterId) =>
        await Post($"api/admin/letters/{letterId}/approve");

    public async Task RejectLetter(string letterId) =>
        await Post($"api/admin/letters/{letterId}/approve");

    public async Task<Letter[]> GetSubmittedLetters() =>
        await Get<Letter[]>("api/admin/letters/submitted");

    public async Task<EventsAndLetters> GetEventLetters(string eventId) =>
        await Get<EventsAndLetters>($"api/admin/letters/events/{eventId}");

    public async Task<Letter[]> GetTemplateLetters(string templateId) =>
        await Get<Letter[]>($"api/admin/letters/templates/{templateId}/letters");

    public Task<AccountAttachment[]> GetAccountAttachments(string accountId) =>
        Get<AccountAttachment[]>($"api/admin/accounts/{accountId}/attachments");

    public async Task<StringResult> Attach(string accountId, Stream data, string fileName, string mediaType) =>
        await PostFile<StringResult>($"api/admin/accounts/{accountId}/attachments/attach", data, fileName, mediaType);

    public Task<IFileInfo> GetAttachment(string attachmentId, string fileName) =>
        Download($"api/attachments/{attachmentId}/{fileName}");

    public Task SaveAttachment(string attachmentId, AccountAttachment attachment) =>
        Post($"api/admin/attachments/{attachmentId}", new { attachment });

    Task<AccountAttachment> IAdminService.GetAttachment(string attachmentId) =>
        Get<AccountAttachment>($"api/admin/attachments/{attachmentId}");

    public Task<AccountAttachment> GetAttachments(string attachmentId) =>
        Get<AccountAttachment>($"api/admin/attachments/{attachmentId}");

    public Task DeleteAttachment(string attachmentId) =>
        Delete($"api/admin/attachments/{attachmentId}");

    public Task<StringResult> DraftEvent() =>
        Post<StringResult>($"api/admin/events/new");

    public async Task<LetterTemplate> GetLetterTemplate(string templateId) =>
        await Get<LetterTemplate>($"api/admin/letters/templates/{templateId}");

    public Task<Letter> GetLetter(string letterId) =>
        Get<Letter>($"api/letters/{letterId}");

    public Task SaveLetter(string letterId, Letter letter) =>
        Post($"api/letters/{letterId}", new { letter });

    public Task<EventsAndLetters> GetEventLetter(string eventId, string letterName) =>
        Get<EventsAndLetters>($"api/letters/events/{eventId}/{letterName}");

    Task<Event> IAdminService.GetEvent(string eventId) =>
        Get<Event>($"api/admin/events/{eventId}");

    public Task SaveEvent(string eventId, Event @event) =>
        Post($"api/admin/events/{eventId}", new { @event });

    public Task DeleteEvent(string eventId) =>
        Delete($"api/admin/events/{eventId}");

    public Task SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone,
        string[] characterIds) =>
        Post($"api/admin/events/{eventId}/attendance/{accountId}", new
        {
            attended,
            moonstone,
            characterIds
        });

    public Task<Dictionary<string, AccountName>> GetAccountNames() =>
        Get<Dictionary<string, AccountName>>($"api/admin/accounts/names");

    public Task<Attendance[]> GetEventAttendances(string eventId) =>
        Get<Attendance[]>($"api/admin/events/{eventId}/attendance");

    public Task<GameState> GetGameState(string lastRevision) =>
        Get<GameState>($"api/mw5e/gameState?lastRevision={lastRevision}");

    public Task<CharacterAndRevision> GetCharacter(string characterId) =>
        Get<CharacterAndRevision>($"api/mw5e/character?characterId={characterId}");

    public Task<CharacterAndRevision> ReviseCharacter(string characterId) =>
        Post<CharacterAndRevision>($"api/mw5e/character/revise?characterId={characterId}");

    public Task<CharacterAndRevision> GetNewCharacter() =>
        Get<CharacterAndRevision>($"api/mw5e/character/new");

    public Task SaveCharacter(CharacterRevision revision) =>
        Post("api/mw5e/character", new { revision });

    public async Task DeleteCharacter(string characterId) =>
        await Delete($"api/mw5e/character?characterId={characterId}");

    public Task<Account[]> GetAccounts() =>
        Get<Account[]>($"api/admin/accounts");

    public Task<StringResult> AddAdminAccount(string fullName, string emailAddress) =>
        Post<StringResult>($"api/admin/accounts/admin", new { fullName, emailAddress });

    public Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state) =>
        Get<CharacterAccountSummary[]>($"api/admin/mw5e/characters?state={state}");

    public Task<CharacterAndRevision> GetMwFifthCharacter(string characterId) =>
        Get<CharacterAndRevision>($"api/admin/mw5e/characters/{characterId}");

    public Task<Account> GetAccount(string accountId) =>
        Get<Account>($"api/admin/accounts/{accountId}");

    public Task<CharacterSummary[]> GetAccountCharacters(string accountId) =>
        Get<CharacterSummary[]>($"api/admin/accounts/{accountId}/characters");

    public Task UpdateAccount(string accountId, string? name, string? location,
        string? phone, DateOnly? birthDate, string? notes) =>
        Post($"api/admin/accounts/{accountId}", new { name, location, phone, birthDate, notes });

    public Task AddAccountRole(string accountId, AccountRole role) =>
        Post($"api/admin/accounts/{accountId}/roles/{role}");

    public Task RemoveAccountRole(string accountId, AccountRole role) =>
        Delete($"api/admin/accounts/{accountId}/roles/{role}");

    public Task<CharacterAndRevision> GetMwFifthCharacterLatest(string characterId) =>
        Get<CharacterAndRevision>($"api/admin/mw5e/characters/{characterId}/latest");

    public Task<CharacterAndRevisions> GetMwFifthCharacterRevisions(string characterId) =>
        Get<CharacterAndRevisions>($"api/admin/mw5e/characters/{characterId}/revisions");

    public Task ApproveMwFifthCharacter(string characterId) =>
        Post($"api/admin/mw5e/characters/{characterId}/approve");

    public Task RejectMwFifthCharacter(string characterId) =>
        Post($"api/admin/mw5e/characters/{characterId}/reject");

    public Task<Dashboard> GetDashboard() =>
        Get<Dashboard>($"api/admin/dashboard");

    public Task<CharacterAndRevision> ReviseMwFifthCharacter(string characterId) =>
        Post<CharacterAndRevision>($"api/admin/mw5e/characters/{characterId}/revise");

    public Task SaveMwFifthCharacter(CharacterRevision revision) =>
        Post($"api/admin/mw5e/characters", new { revision });

    public Task DeleteMwFifthCharacter(string characterId) =>
        Delete($"api/admin/mw5e/characters/{characterId}");

    Task<EventAndLetters[]> IAdminService.GetEvents() =>
        Get<EventAndLetters[]>($"api/admin/events");
}