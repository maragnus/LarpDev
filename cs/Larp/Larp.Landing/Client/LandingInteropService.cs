using System.Net.Http.Json;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;

namespace Larp.Landing.Client;

public class LandingInteropService : ILandingService, IMwFifthGameService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LandingInteropService> _logger;

    public LandingInteropService(HttpClient httpClient, ILogger<LandingInteropService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    private async Task<TItem?> Get<TItem>(string uri) where TItem : new() =>
        await _httpClient.GetFromJsonAsync<TItem>(uri);

    private async Task<TItem[]> GetArray<TItem>(string uri) =>
        await _httpClient.GetFromJsonAsync<TItem[]>(uri) ?? Array.Empty<TItem>();
    
    public async Task<Game[]> GetGames() =>
        await GetArray<Game>("api/larp/games");

    public async Task<CharacterSummary[]> GetCharacters() =>
        await GetArray<CharacterSummary>("api/larp/characters");

    public Task<GameState?> GetGameState(string lastRevision) =>
        Get<GameState>($"api/larp/mw5e/gameState?lastRevision={lastRevision}");

    public Task<Character?> GetCharacter(string characterId) =>
        Get<Character>($"api/larp/mw5e/{characterId}");
}