using JetBrains.Annotations;
using Larp.Data;
using Larp.Landing.Shared.Messages;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("/api")]
public interface ILandingService
{
    [ApiGet("auth/login")]
    Task<Result> Login(string email, string origin);

    [ApiGet("auth/confirm")]
    Task<StringResult> Confirm(string email, string token);

    [ApiGet("auto/validate")]
    Task<Result> Validate(string token);
    
    [ApiGet("games")]
    Task<Game[]> GetGames();

    [ApiGet("characters")]
    Task<CharacterSummary[]> GetCharacters();
}