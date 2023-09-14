using Larp.Data.MwFifth;
using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api/admin")]
public interface IAdminService
{
    [ApiGet("dashboard"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<Dashboard> GetDashboard();

    [ApiGet("accounts"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<Account[]> GetAccounts(AccountState accountState);

    [ApiGet("accounts/{accountId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<Account> GetAccount(string accountId);

    [ApiGet("accounts/{accountId}/characters"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<CharacterSummary[]> GetAccountCharacters(string accountId);

    [ApiPost("accounts/{accountId}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task UpdateAccount(string accountId, string? name, string? location, string? phone, DateOnly? birthDate,
        string? notes, int? discount);

    [ApiPost("accounts/{accountId}/roles/{role}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task AddAccountRole(string accountId, AccountRole role);

    [ApiDelete("accounts/{accountId}/roles/{role}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task RemoveAccountRole(string accountId, AccountRole role);

    [ApiPost("accounts/admin"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<StringResult> AddAdminAccount(string fullName, string emailAddress);

    [ApiPost("accounts/add"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<StringResult> AddAccount(string fullName, string emailAddress);

    [ApiGet("mw5e/characters"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state);

    [ApiGet("mw5e/characters/{characterId}"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<CharacterAndRevision> GetMwFifthCharacter(string characterId);

    /// <summary>Returns the best revision to start a draft (in order of Draft, Review, Live)</summary>
    [ApiGet("mw5e/characters/{characterId}/latest"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<CharacterAndRevision> GetMwFifthCharacterLatest(string characterId);

    [ApiGet("mw5e/characters/{characterId}/revisions"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<CharacterAndRevisions> GetMwFifthCharacterRevisions(string characterId);

    [ApiPost("mw5e/characters/{revisionId}/approve"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task ApproveMwFifthCharacter(string revisionId);

    [ApiPost("mw5e/characters/{revisionId}/reject"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task RejectMwFifthCharacter(string revisionId, string? reviewerNotes);

    [ApiPost("mw5e/characters/{revisionId}/retire"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task RetireMwFifthCharacter(string revisionId);

    [ApiPost("mw5e/characters/{revisionId}/unretire"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task UnretireMwFifthCharacter(string revisionId);

    [ApiPost("mw5e/characters/{revisionId}/revise"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<CharacterAndRevision> ReviseMwFifthCharacter(string revisionId);

    [ApiPost("mw5e/characters"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveMwFifthCharacter(CharacterRevision revision);

    [ApiDelete("mw5e/characters/{revisionId}"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task DeleteMwFifthCharacter(string revisionId);

    [ApiPost("mw5e/characters/{characterId}/move"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task MoveMwFifthCharacter(string characterId, string newAccountId);

    [ApiGet("events"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<EventAndLetters[]> GetEvents();

    [ApiGet("events/{eventId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<Event> GetEvent(string eventId);

    [ApiPost("events/{eventId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task SaveEvent(string eventId, Event @event);

    [ApiDelete("events/{eventId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task DeleteEvent(string eventId);

    [ApiPost("events/{eventId}/attendance/{accountId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task SetEventAttendance(string eventId, string accountId, bool attended, int? moonstone1,
        int? moonstone2, int? paid, int? cost, string[] characterIds);

    [ApiGet("accounts/names")]
    Task<Dictionary<string, AccountName>> GetAccountNames();

    [ApiGet("events/{eventId}/attendance"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<Attendance[]> GetEventAttendances(string eventId);

    [ApiGet("account/{accountId}/attendance"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task<EventAttendanceList> GetAccountAttendances(string accountId);

    [ApiPost("data/import"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<StringResult> Import(Stream data);

    [ApiGet("data/export"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    [ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> Export();

    [ApiGet("data/export/occupations"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    [ApiContentType("application/json")]
    Task<IFileInfo> ExportOccupations();

    [ApiGet("letters/events/{eventId}/export"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    [ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> ExportLetters(string eventId);

    [ApiPost("accounts/merge"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task MergeAccounts(string fromAccountId, string toAccountId);

    [ApiPost("accounts/{accountId}/emails"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task AddAccountEmail(string accountId, string email);

    [ApiDelete("accounts/{accountId}/emails"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task RemoveAccountEmail(string accountId, string email);

    [ApiPost("letters/templates/new"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<LetterTemplate> DraftLetterTemplate();

    [ApiPost("letters/templates/{templateId}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task SaveLetterTemplate(string templateId, LetterTemplate template);

    [ApiGet("letters/templates"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<LetterTemplate[]> GetLetterTemplates();

    [ApiGet("letters/templates/names"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<LetterTemplate[]> GetLetterTemplateNames();

    [ApiGet("letters/templates/{templateId}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<LetterTemplate> GetLetterTemplate(string templateId);

    [ApiPost("letters/{letterId}/approve"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task ApproveLetter(string letterId);

    [ApiPost("letters/{letterId}/reject"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task RejectLetter(string letterId);

    [ApiPost("letters/{letterId}/unapprove"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task UnapproveLetter(string letterId);

    [ApiGet("letters/submitted"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<Letter[]> GetSubmittedLetters();

    [ApiGet("letters/events/{eventId}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<EventsAndLetters> GetEventLetters(string eventId);

    [ApiGet("letters/templates/{templateId}/letters"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<Letter[]> GetTemplateLetters(string templateId);

    [ApiGet("accounts/{accountId}/attachments"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<AccountAttachment[]> GetAccountAttachments(string accountId);

    [ApiPost("accounts/{accountId}/attachments/attach"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<StringResult> Attach(string accountId, Stream data, string fileName, string mediaType);

    [ApiPost("attachments/{attachmentId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task SaveAttachment(string attachmentId, AccountAttachment attachment);

    [ApiGet("attachments/{attachmentId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<AccountAttachment> GetAttachment(string attachmentId);

    [ApiDelete("attachments/{attachmentId}"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task DeleteAttachment(string attachmentId);

    [ApiPost("events/new")]
    Task<Event> DraftEvent();

    [ApiPost("mw5e/characters/{characterId}/notes"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task SetMwFifthCharacterNotes(string characterId, string? notes);

    [ApiPost("accounts/{accountId}/notes/preregistration"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task SetAccountPreregistrationNotes(string accountId, string? notes);

    [ApiPost("accounts/{accountId}/notes/admin"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task SetAccountAdminNotes(string accountId, string? notes);

    [ApiGet("events/{eventId}/notes"), ApiAuthenticated(AccountRoles.AdminAccess)]
    Task<PreregistrationNotes> GetEventNotes(string eventId);

    [ApiPost("accounts/{accountId}/uninvite"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task UninviteAccount(string accountId);

    [ApiPost("accounts/{accountId}/archive"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task ArchiveAccount(string accountId);

    [ApiPost("accounts/{accountId}/restore"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task RestoreAccount(string accountId);

    [ApiGet("log"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<string[]> GetLog();

    [ApiPost("data/reseed"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task ReseedData();

    [ApiDelete("data/characters/unused"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task DeleteCharactersUnused();

    [ApiPost("mw5e/occupations"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveOccupations(Occupation[] occupations);

    [ApiPost("mw5e/spells"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveSpells(Spell[] spells);

    [ApiPost("mw5e/skills"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveSkills(SkillDefinition[] skills);

    [ApiPost("mw5e/advantages"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveAdvantages(Vantage[] vantages);

    [ApiPost("mw5e/disadvantages"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveDisadvantages(Vantage[] vantages);

    [ApiPost("mw5e/chapters"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveChapters(HomeChapter[] chapters);

    [ApiPost("mw5e/religions"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SaveReligions(Religion[] religions);

    [ApiPost("mw5e/terms"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task SetTerm(string name, string? summary, string? description);

    [ApiDelete("mw5e/terms"), ApiAuthenticated(AccountRoles.MwFifthGameMaster)]
    Task DeleteTerm(string name);

    [ApiGet("accounts/{accountId}/citations"), ApiAuthenticated(AccountRoles.CitationAccess)]
    Task<Citation[]> GetCitations(string accountId);

    [ApiPost("citations"), ApiAuthenticated(AccountRoles.CitationAccess)]
    Task UpdateCitations(Citation citation);

    [ApiPost("citations/{citationId}"), ApiAuthenticated(AccountRoles.CitationAccess)]
    Task SetCitationState(string citationId, CitationState state);

    [ApiPost("letters/{letterId}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task SaveLetter(string letterId, Letter letter);

    [ApiGet("letters/{letterId}"), ApiAuthenticated(AccountRoles.AccountAdmin)]
    Task<EventsAndLetters> GetLetter(string letterId);

    [ApiPost("transactions/update"), ApiAuthenticated(AccountRoles.FinanceAccess)]
    Task UpdateTransactions();
}

public class Dashboard
{
    public int MwFifthCharacters { get; set; }
    public int MwFifthReview { get; set; }
    public int Accounts { get; set; }
    public int VerifiedAccounts { get; set; }
    public int ReviewLetters { get; set; }
}