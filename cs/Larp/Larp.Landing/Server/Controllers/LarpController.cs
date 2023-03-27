using System.Text.Json;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Larp.Landing.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LarpController : ControllerBase
{
    private readonly ILandingService _landingService;
    private readonly IMwFifthService _mwFifthService;

    public LarpController(ILandingService landingService, IMwFifthService mwFifthService)
    {
        _landingService = landingService;
        _mwFifthService = mwFifthService;
    }

    public record LoginRequest(string Email, string Origin);
    public record ConfirmRequest(string Email, string Token);
    public record ValidateRequest(string Token);

    [HttpPost("auth/login")]
    public async Task<Result> Login([FromBody]LoginRequest body) =>
        await _landingService.Login(body.Email, body.Origin);

    [HttpPost("auth/confirm")]
    public async Task<StringResult> Confirm([FromBody]ConfirmRequest body) =>
        await _landingService.Confirm(body.Email, body.Token);

    [HttpPost("auth/validate")]
    public async Task<Result> Validate([FromBody]ValidateRequest body) =>
        await _landingService.Validate(body.Token);

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
    public async Task<GameState?> GetGameState([FromQuery] string? lastRevision)
    {
        return await _mwFifthService.GetGameState(lastRevision ?? "");
    }

    [HttpGet("mw5e/characters/:characterId")]
    public async Task<Character?> GetCharacter([FromRoute] string characterId)
    {
        return await _mwFifthService.GetCharacter(characterId);
    }

    public async Task<Character> GetNewCharacter()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveCharacter(Character character)
    {
        throw new NotImplementedException();
    }
}