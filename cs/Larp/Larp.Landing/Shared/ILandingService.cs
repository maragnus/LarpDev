using JetBrains.Annotations;
using Larp.Data;
using Larp.Landing.Shared.Messages;

namespace Larp.Landing.Shared;

[PublicAPI]
public interface ILandingService
{
    [ApiPath("auth/login")]
    Task<Result> Login(string email, string origin);

    [ApiPath("auth/confirm")]
    Task<StringResult> Confirm(string email, string token);

    [ApiPath("auto/validate")]
    Task<Result> Validate(string token);
    
    [ApiPath("games")]
    Task<Game[]> GetGames();

    [ApiPath("characters")]
    Task<CharacterSummary[]> GetCharacters();
}