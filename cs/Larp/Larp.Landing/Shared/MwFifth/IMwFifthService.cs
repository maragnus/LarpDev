﻿using JetBrains.Annotations;
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
    Task<Character> GetCharacter(string characterId);

    [ApiGet("character/new"), ApiAuthenticated]
    Task<Character> GetNewCharacter();
    
    [ApiPost("character"), ApiAuthenticated]
    Task<Result> SaveCharacter(Character character);
}