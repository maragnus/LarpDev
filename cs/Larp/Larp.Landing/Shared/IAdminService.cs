using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared.Messages;
using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api/admin")]
public interface IAdminService
{
    [ApiGet("dashboard"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<Dashboard> GetDashboard();

    [ApiGet("accounts"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<Account[]> GetAccounts();

    [ApiGet("accounts/{accountId}"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<Account> GetAccount(string accountId);

    [ApiGet("accounts/{accountId}/characters"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<CharacterSummary[]> GetAccountCharacters(string accountId);

    [ApiPost("accounts/{accountId}"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task UpdateAccount(string accountId, string? name, string? location, string? phone, DateOnly? birthDate,
        string? notes);

    [ApiPost("accounts/{accountId}/roles/{role}"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task AddAccountRole(string accountId, AccountRole role);

    [ApiDelete("accounts/{accountId}/roles/{role}"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task RemoveAccountRole(string accountId, AccountRole role);

    [ApiPost("accounts/admin"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<StringResult> AddAdminAccount(string fullName, string emailAddress);

    [ApiGet("mw5e/characters"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state);

    [ApiGet("mw5e/characters/{characterId}"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<CharacterAndRevision> GetMwFifthCharacter(string characterId);

    /// <summary>Returns the best revision to start a draft (in order of Draft, Review, Live)</summary>
    [ApiGet("mw5e/characters/{characterId}/latest"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<CharacterAndRevision> GetMwFifthCharacterLatest(string characterId);

    [ApiGet("mw5e/characters/{characterId}/revisions"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<CharacterAndRevisions> GetMwFifthCharacterRevisions(string characterId);

    [ApiPost("mw5e/characters/{characterId}/approve"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task ApproveMwFifthCharacter(string characterId);

    [ApiPost("mw5e/characters/{characterId}/reject"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task RejectMwFifthCharacter(string characterId);

    [ApiPost("mw5e/characters/{characterId}/revise"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<CharacterAndRevision> ReviseMwFifthCharacter(string characterId);

    [ApiPost("mw5e/characters"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task SaveMwFifthCharacter(CharacterRevision revision);

    [ApiDelete("mw5e/characters/{characterId}"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task DeleteMwFifthCharacter(string characterId);
    
    [ApiPost("mw5e/characters/{characterId}/move"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task MoveMwFifthCharacter(string characterId, string newAccountId);

    [ApiGet("events"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<EventAndLetters[]> GetEvents();

    [ApiGet("events/{eventId}"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<Event> GetEvent(string eventId);

    [ApiPost("events/{eventId}"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task SaveEvent(string eventId, Event @event);

    [ApiDelete("events/{eventId}"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task DeleteEvent(string eventId);

    [ApiPost("events/{eventId}/attendance/{accountId}"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone, string[] characterIds);

    [ApiGet("accounts/names")]
    Task<AccountName[]> GetAccountNames();

    [ApiGet("events/{eventId}/attendance"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<Attendance[]> GetEventAttendances(string eventId);

    [ApiPost("data/import"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<StringResult> Import(Stream data);

    [ApiGet("data/export"), ApiAuthenticated(AccountRole.AccountAdmin)]
    [ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> Export();

    [ApiGet("letters/events/{eventId}/export"), ApiAuthenticated(AccountRole.AccountAdmin)]
    [ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> ExportLetters(string eventId);

    [ApiPost("accounts/merge"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task MergeAccounts(string fromAccountId, string toAccountId);

    [ApiPost("accounts/{accountId}/emails"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task AddAccountEmail(string accountId, string email);

    [ApiDelete("accounts/{accountId}/emails"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task RemoveAccountEmail(string accountId, string email);

    [ApiPost("letters/templates/new"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<LetterTemplate> DraftLetterTemplate();

    [ApiPost("letters/templates/{templateId}"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task SaveLetterTemplate(string templateId, LetterTemplate template);
    
    [ApiGet("letters/templates"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<LetterTemplate[]> GetLetterTemplates();
    
    [ApiGet("letters/templates/names"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<LetterTemplate[]> GetLetterTemplateNames();
    
    [ApiGet("letters/templates/{templateId}"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<LetterTemplate> GetLetterTemplate(string templateId);
    
    [ApiGet("letters"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<Letter[]> GetLetters();
    
    [ApiPost("letters/{letterId}/approve"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task ApproveLetter(string letterId);
    
    [ApiPost("letters/{letterId}/reject"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task RejectLetter(string letterId);

    [ApiGet("letters/submitted"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<Letter[]> GetSubmittedLetters();
    
    [ApiGet("letters/events/{eventId}"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<EventsAndLetters> GetEventLetters(string eventId);
    
    [ApiGet("letters/templates/{templateId}/letters"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task<Letter[]> GetTemplateLetters(string templateId);
}

public class Dashboard
{
    public int MwFifthCharacters { get; set; }
    public int MwFifthReview { get; set; }
    public int Accounts { get; set; }
    public int VerifiedAccounts { get; set; }
    public int ReviewLetters { get; set; }
}