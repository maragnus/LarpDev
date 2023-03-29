using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using Larp.Landing.Shared.MwFifth;

namespace Larp.Landing.Client.Services;

public class LandingServiceClient : RestClient, ILandingService, IMwFifthService
{
    public LandingServiceClient(HttpClient httpClient, ILogger<LandingServiceClient> logger) 
        : base(httpClient, logger)
    {
    }

    public async Task<Result> Login(string email, string deviceName) =>
        await Post<Result>("api/auth/login", new { Email = email, DeviceName = deviceName });

    public async Task<StringResult> Confirm(string email, string token, string deviceName) =>
        await Post<StringResult>("api/auth/confirm",
            new { Email = email, Token = token, DeviceName = deviceName });

    public async Task<Result> Logout() =>
        await Post<Result>("api/auth/logout");

    public async Task<Result> Validate() =>
        await Post<Result>("api/auth/validate");

    public async Task<Game[]> GetGames() =>
        await GetArray<Game>("api/larp/games");

    public async Task<CharacterSummary[]> GetCharacters() =>
        await GetArray<CharacterSummary>("api/larp/characters");

    public Task<GameState> GetGameState(string lastRevision) =>
        Get<GameState>($"api/mw5e/gameState?lastRevision={lastRevision}");

    public Task<Character> GetCharacter(string characterId) =>
        Get<Character>($"api/mw5e/character?characterId={characterId}");

    public Task<Character> GetNewCharacter() =>
        Get<Character>($"api/mw5e/character/new")!;

    public Task<StringResult> SaveCharacter(Character character) =>
        Post<StringResult>("api/mw5e/character", new { character });
}