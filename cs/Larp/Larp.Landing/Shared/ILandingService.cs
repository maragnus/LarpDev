using JetBrains.Annotations;
using Larp.Data;
using Larp.Data.MwFifth;

namespace Larp.Landing.Shared;

[PublicAPI]
public interface ILandingService
{
    [ApiPath("games")]
    Task<Game[]> GetGames();

    [ApiPath("characters")]
    Task<CharacterSummary[]> GetCharacters();
}

[PublicAPI]
public interface IMwFifthGameService
{
    [ApiPath("mw5e/gameState")]
    Task<GameState?> GetGameState(string lastRevision);

    [ApiPath("mw5e/character/:characterId")]
    Task<Character?> GetCharacter(string characterId);
}