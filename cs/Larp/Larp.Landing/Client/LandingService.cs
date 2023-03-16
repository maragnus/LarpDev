using Blazored.LocalStorage;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;

namespace Larp.Landing.Client;

public class MwFifthService
{
    private readonly IMwFifthGameService _mwFifth;
    private readonly ILocalStorageService _localStorage;
    private readonly DataCacheService _dataCache;
    private readonly LandingService _landingService;

    private const string DraftCharacterStorageKey = "MwFifth.NewCharacter";
    
    public MwFifthService(LandingService landingService, IMwFifthGameService mwFifth, ILocalStorageService localStorage, DataCacheService dataCache)
    {
        _mwFifth = mwFifth;
        _localStorage = localStorage;
        _dataCache = dataCache;
        _landingService = landingService;
    }

    public async Task<Character?> GetCharacter(string? characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
            return null;

        return await _mwFifth.GetCharacter(characterId);
    }

    public Game Game => _landingService.Games[GameState.GameName];
    public GameState GameState { get; private set; } = null!;

    public async Task Refresh()
    {
        await GetMwFifthGameState();
    }
    
    private async Task GetMwFifthGameState()
    {
        GameState = await _dataCache.CacheGameState<GameState>(GameState.GameName,
            async revision => await _mwFifth.GetGameState(revision));
    }

    public async Task StoreDraftCharacter(Character character)
    {
        await _localStorage.SetItemAsync(DraftCharacterStorageKey, character);
    }
    
    public async Task<Character> GetDraftCharacter()
    {
        try
        {
            if (!await _localStorage.ContainKeyAsync(DraftCharacterStorageKey))
                return new Character();

            return await _localStorage.GetItemAsync<Character>(DraftCharacterStorageKey);
        }
        catch
        {
            return new Character();
        }
    }
}

public class LandingService
{
    private readonly ILandingService _landing;
    private readonly DataCacheService _dataCache;
    private readonly ILogger<LandingService> _logger;

    private const int GameStateUpdateFrequencyHours = 4;

    public LandingService(ILandingService landing,
        IMwFifthGameService mwFifth,
        DataCacheService dataCache,
        ILogger<LandingService> logger, ILocalStorageService localStorage)
    {
        _landing = landing;
        _dataCache = dataCache;
        _logger = logger;
        MwFifth = new MwFifthService(this, mwFifth, localStorage, dataCache);
    }

    public IReadOnlyDictionary<string, Game> Games { get; private set; } = null!;
    public MwFifthService MwFifth { get; }
    public bool IsLoggedIn => false;

    public async Task<CharacterSummary[]> GetCharacters() => await _landing.GetCharacters();

    public async Task Refresh()
    {
        _logger.LogInformation("Refresh starting...");
        await GetGames();
        await MwFifth.Refresh();
        _logger.LogInformation("Refresh complete");
    }

    private async Task GetGames()
    {
        Games = await _dataCache
            .CacheItem(nameof(Games), TimeSpan.FromHours(GameStateUpdateFrequencyHours), async () =>
            {
                var games = await _landing.GetGames();
                return games.ToDictionary(x => x.Name);
            });
    }

    public async Task Logout()
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}