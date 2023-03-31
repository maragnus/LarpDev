using Larp.Data;
using Larp.Landing.Shared.Messages;
using Microsoft.Extensions.FileProviders;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api")]
public interface ILandingService
{
    [ApiPost("auth/login")]
    Task<Result> Login(string email, string deviceName);

    [ApiPost("auth/confirm")]
    Task<StringResult> Confirm(string email, string token, string deviceName);

    [ApiPost("auth/logout")]
    Task<Result> Logout();
    
    [ApiPost("auth/validate"), ApiAuthenticated]
    Task<Result> Validate();

    [ApiGet("larp/games")]
    Task<Game[]> GetGames();

    [ApiGet("larp/characters"), ApiAuthenticated]
    Task<CharacterSummary[]> GetCharacters();

    [ApiGet("account")]
    Task<Account> GetAccount();

    [ApiPost("account/email")]
    Task AccountEmailAdd(string email);

    [ApiDelete("account/email")]
    Task AccountEmailRemove(string email);

    [ApiPost("account/email/preferred")]
    Task AccountEmailPreferred(string email);

    [ApiPost("account")]
    Task AccountUpdate(string? fullName, string? location, string? phone, string? allergies, DateOnly? birthDate);
}