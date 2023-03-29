using JetBrains.Annotations;
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
    
    [ApiGet("export"), ApiContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Task<IFileInfo> Export();
}