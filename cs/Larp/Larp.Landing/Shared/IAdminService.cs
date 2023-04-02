using Larp.Data;
using MwFifthCharacter = Larp.Data.MwFifth.Character;
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

    [ApiGet("export"), ApiAuthenticated(AccountRole.AccountAdmin)]
    [ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> Export();
    
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
    
    [ApiGet("mw5e/characters"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state);

    [ApiGet("mw5e/characters/{characterId}"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<MwFifthCharacter> GetMwFifthCharacter(string characterId);

    [ApiGet("mw5e/characters/{characterId}/latest"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<MwFifthCharacter> GetMwFifthCharacterLatest(string characterId);

    [ApiGet("mw5e/characters/{characterId}/revisions"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<MwFifthCharacter[]> GetMwFifthCharacterRevisions(string characterId);

    [ApiPost("mw5e/characters/{characterId}/approve"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task ApproveMwFifthCharacter(string characterId);

    [ApiPost("mw5e/characters/{characterId}/reject"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task RejectMwFifthCharacter(string characterId);

    [ApiPost("mw5e/characters/{characterId}/revise"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task<MwFifthCharacter> ReviseMwFifthCharacter(string characterId);

    [ApiPost("mw5e/characters/{characterId}"), ApiAuthenticated(AccountRole.MwFifthGameMaster)]
    Task SaveMwFifthCharacter(MwFifthCharacter character);
}

public class Dashboard
{
    public int MwFifthCharacters { get; set; }
    public int MwFifthReview { get; set; }
    public int Accounts { get; set; }
}