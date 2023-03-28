using JetBrains.Annotations;
using Larp.Data.MwFifth;

namespace Larp.Landing.Shared;

[PublicAPI]
[ApiRoot("api/mw5e")]
public interface IMwFifthService
{
    [ApiGet("gameState")]
    Task<GameState?> GetGameState(string lastRevision);

    [ApiGet("character/:characterId")]
    Task<Character?> GetCharacter(string characterId);

    [ApiGet("character/new")]
    Task<Character> GetNewCharacter();
    
    [ApiPost("character/:characterId")]
    Task<bool> SaveCharacter(Character character);
}