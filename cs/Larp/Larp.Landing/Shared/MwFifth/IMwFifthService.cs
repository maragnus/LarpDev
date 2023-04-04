using Larp.Data.MwFifth;
using Larp.Landing.Shared.Messages;

namespace Larp.Landing.Shared.MwFifth;

[PublicAPI]
[ApiRoot("api/mw5e")]
public interface IMwFifthService
{
    [ApiGet("gameState")]
    Task<GameState> GetGameState(string lastRevision);
    
    [ApiGet("character"), ApiAuthenticated]
    Task<CharacterAndRevision> GetCharacter(string characterId);
    
    [ApiPost("character/revise"), ApiAuthenticated]
    Task<CharacterAndRevision> ReviseCharacter(string characterId);

    [ApiGet("character/new"), ApiAuthenticated]
    Task<CharacterAndRevision> GetNewCharacter();
    
    [ApiPost("character"), ApiAuthenticated]
    Task SaveCharacter(CharacterRevision revision);
    
    [ApiDelete("character"), ApiAuthenticated]
    Task DeleteCharacter(string characterId);
}