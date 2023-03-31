using Larp.Data;
using Larp.Data.MwFifth;
using MwFifthCharacter = Larp.Data.MwFifth.Character;
using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api/admin")]
public interface IAdminService
{
    [ApiGet("accounts"), ApiAuthenticated]
    Task<Account[]> GetAccounts();

    [ApiGet("export"), ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> Export();

    [ApiGet("mw5e/characters"), ApiAuthenticated]
    Task<CharacterAccountSummary[]> GetMwFifthCharacters(CharacterState state);

    [ApiGet("mw5e/characters/{characterId}"), ApiAuthenticated]
    Task<MwFifthCharacter> GetMwFifthCharacter(string characterId);

    [ApiGet("accounts/{accountId}"), ApiAuthenticated]
    Task<Account> GetAccount(string accountId);
    
    [ApiGet("accounts/{accountId}/characters"), ApiAuthenticated]
    Task<CharacterSummary[]> GetAccountCharacters(string accountId);

    [ApiPost("accounts/{accountId}"), ApiAuthenticated]
    Task UpdateAccount(string accountId, string? name, string? location, string? phone, DateOnly? birthDate, string? notes);
}