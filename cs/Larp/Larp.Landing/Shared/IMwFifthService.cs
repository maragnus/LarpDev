using JetBrains.Annotations;
using Larp.Data.MwFifth;

namespace Larp.Landing.Shared;

[PublicAPI]
public interface IMwFifthService
{
    [ApiPath("mw5e/gameState")]
    Task<GameState?> GetGameState(string lastRevision);

    [ApiPath("mw5e/character/:characterId")]
    Task<Character?> GetCharacter(string characterId);

    [ApiPath("mw5e/character/new")]
    Task<Character> GetNewCharacter();
    
    [ApiPath("mw5e/character/:characterId")]
    Task<bool> SaveCharacter(Character character);
}