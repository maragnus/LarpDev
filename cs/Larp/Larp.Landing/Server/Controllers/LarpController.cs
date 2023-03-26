using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Larp.Landing.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LarpController : ControllerBase, ILandingService, IMwFifthGameService
{
    private readonly ILandingService _landingService;
    private readonly IMwFifthGameService _mwFifthGameService;

    public LarpController(ILandingService landingService, IMwFifthGameService mwFifthGameService)
    {
        _landingService = landingService;
        _mwFifthGameService = mwFifthGameService;
    }

    [HttpGet("games")]
    public async Task<Game[]> GetGames()
    {
        return await _landingService.GetGames();
    }

    [HttpGet("characters")]
    public async Task<CharacterSummary[]> GetCharacters()
    {
        return await _landingService.GetCharacters();
    }

    [HttpGet("mw5e/gameState")]
    public async Task<GameState?> GetGameState([FromQuery]string? lastRevision)
    {
        return await _mwFifthGameService.GetGameState(lastRevision ?? "");
    }

    [HttpGet("mw5e/characters/:characterId")]
    public async Task<Character?> GetCharacter([FromRoute]string characterId)
    {
        return await _mwFifthGameService.GetCharacter(characterId);
    }
}