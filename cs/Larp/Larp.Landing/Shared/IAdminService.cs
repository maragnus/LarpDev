using Larp.Data;
using Larp.Data.MwFifth;
using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Shared;

public record CharacterAccountSummary(
    string CharacterId,
    string AccountId,
    CharacterState State,
    string AccountName,
    string AccountEmails,
    string CharacterName,
    string HomeChapter,
    string Occupation,
    int Level
);

[PublicAPI]
[ApiRoot("/api/admin")]
public interface IAdminService
{
    
    [ApiGet("accounts"), ApiAuthenticated]
    Task<Account[]> GetAccounts();
        
    [ApiGet("export"), ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> Export();

    Task<CharacterAccountSummary[]> GetMwFifthCharacters();
}