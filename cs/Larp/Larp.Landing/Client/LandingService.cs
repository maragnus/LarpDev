using Blazored.LocalStorage;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;

namespace Larp.Landing.Client;

public class LandingService
{
    private readonly ILandingService _landing;
    private readonly IMwFifthGameService _mwFifth;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<LandingService> _logger;

    private const string MwFifthGameState = "MwFifthGameState";
    private const int GameStateUpdateFrequencyHours = 2;
    
    public LandingService(ILandingService landing, IMwFifthGameService mwFifth, ILocalStorageService localStorage, ILogger<LandingService> logger)
    {
        _landing = landing;
        _mwFifth = mwFifth;
        _localStorage = localStorage;
        _logger = logger;
    }

    public async Task Refresh()
    {
        _logger.LogInformation("Refresh starting...");
        await GetMwFifthGameState();
        _logger.LogInformation("Refresh complete");
    }

    public async Task<CharacterSummary[]> GetCharacters() => await _landing.GetCharacters();

    public async Task<GameState> GetMwFifthGameState() =>
        await GetGameState<GameState>(MwFifthGameState, async revision => await _mwFifth.GetGameState(revision));

    private async Task<TGameState> GetGameState<TGameState> (string key, Func<string, Task<TGameState?>> fetch)
        where TGameState : GameStateBase
    {
        var timeStampKey = $"{key}.TimeStamp";
        
        if (!await _localStorage.ContainKeyAsync(key))
        {
            _logger.LogInformation("GameState is not present and will be fetched");
            var state = await fetch(string.Empty);
            await _localStorage.SetItemAsync(key, state);
            await _localStorage.SetItemAsync(timeStampKey, DateTime.UtcNow);
            return state!;
        }

        var cachedState = await _localStorage.GetItemAsync<TGameState>(key);
        var lastUpdate = await _localStorage.GetItemAsync<DateTime>(timeStampKey);

        _logger.LogInformation("GameState was cached {LastCache}", lastUpdate.ToLocalTime());

        // Only update every few hours
        if (DateTime.UtcNow - lastUpdate < TimeSpan.FromHours(GameStateUpdateFrequencyHours))
            return cachedState;
        
        try
        {
            var newState = await fetch(cachedState.Revision);

            if (newState == null)
            {
                _logger.LogInformation("GameState is current up-to-date");
                return cachedState;
            }

            _logger.LogInformation("GameState updated with new revision {OldRevision} to {NewRevision}", cachedState.Revision, newState.Revision);
            await _localStorage.SetItemAsync(key, newState);
            await _localStorage.SetItemAsync(timeStampKey, DateTime.UtcNow);
            return newState;
        }
        
        // If update fails, log it but return the cached copy
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GameState failed to update");
            return cachedState;
        }
    }
}