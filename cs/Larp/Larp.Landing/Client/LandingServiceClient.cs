using System.Net.Http.Json;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;

namespace Larp.Landing.Client;

public class LandingServiceClient : ILandingService, IMwFifthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LandingServiceClient> _logger;

    public LandingServiceClient(HttpClient httpClient, ILogger<LandingServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    private async Task<TResult?> Get<TResult>(string uri) where TResult : new() =>
        await _httpClient.GetFromJsonAsync<TResult>(uri);

    private async Task Post<TBody>(string uri, TBody item) where TBody : new() =>
        await _httpClient.PostAsJsonAsync(uri, item);

    private async Task<TResult> Post<TBody, TResult>(string uri, TBody item) where TBody : new()
    {
        var response = await _httpClient.PostAsJsonAsync(uri, item);
        return (await response.Content.ReadFromJsonAsync<TResult>())!;
    }

    private async Task<TItem[]> GetArray<TItem>(string uri) =>
        await _httpClient.GetFromJsonAsync<TItem[]>(uri) ?? Array.Empty<TItem>();

    public async Task<Result> Login(string email, string origin) =>
        await Post<object, Result>("api/auth/login", new { Email = email, Origin = origin });

    public async Task<StringResult> Confirm(string email, string token) =>
        await Post<object, StringResult>("api/auth/login", new { Email = email, Token = token });

    public async Task<Result> Validate(string token) =>
        await Post<object, Result>("api/auth/token", new { Token = token });

    public async Task<Game[]> GetGames() =>
        await GetArray<Game>("api/larp/games");

    public async Task<CharacterSummary[]> GetCharacters() =>
        await GetArray<CharacterSummary>("api/larp/characters");

    public Task<GameState?> GetGameState(string lastRevision) =>
        Get<GameState>($"api/larp/mw5e/gameState?lastRevision={lastRevision}");

    public Task<Character?> GetCharacter(string characterId) =>
        Get<Character>($"api/larp/mw5e/{characterId}");

    public Task<Character> GetNewCharacter() =>
        Get<Character>($"api/larp/mw5e/new")!;

    public Task<bool> SaveCharacter(Character character) =>
        Post<Character, bool>("api/larp/mw5e/save", character);
}