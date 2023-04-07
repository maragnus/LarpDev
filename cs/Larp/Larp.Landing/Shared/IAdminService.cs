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
    Task<Event[]> GetEvents();

    [ApiGet("events/{eventId}"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task<Event> GetEvent(string eventId);

    [ApiPost("events/{eventId}"), ApiAuthenticated(AccountRole.AdminAccess)]
    Task SaveEvent(string eventId, string gameId, string? title, string? type, string? location, DateTimeOffset date,
        bool rsvp, bool hidden, EventComponent[] components);

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

    [ApiPost("accounts/merge"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task MergeAccounts(string fromAccountId, string toAccountId);

    [ApiPost("accounts/{accountId}/emails"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task AddAccountEmail(string accountId, string email);

    [ApiDelete("accounts/{accountId}/emails"), ApiAuthenticated(AccountRole.AccountAdmin)]
    Task RemoveAccountEmail(string accountId, string email);
}

public class Dashboard
{
    public int MwFifthCharacters { get; set; }
    public int MwFifthReview { get; set; }
    public int Accounts { get; set; }
    public int VerifiedAccounts { get; set; }
}