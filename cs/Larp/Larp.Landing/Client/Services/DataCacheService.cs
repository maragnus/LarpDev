using Blazored.LocalStorage;
using Larp.Data.MwFifth;

namespace Larp.Landing.Client.Services;

public class DataCacheService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<DataCacheService> _logger;

    private const double GameStateUpdateFrequencyHours = 2;

    public DataCacheService(ILocalStorageService localStorage, ILogger<DataCacheService> logger)
    {
        _localStorage = localStorage;
        _logger = logger;
    }

    public async Task<TCacheItem> CacheItem<TCacheItem>(string keyName, TimeSpan cacheDuration, Func<Task<TCacheItem>> updateCache)
    {
        var expiryKeyName = $"{keyName}.LastUpdated";

        async Task<TCacheItem> UpdateCache()
        {
            var newItem = await updateCache();
            await _localStorage.SetItemAsync(keyName, newItem);
            await _localStorage.SetItemAsync(expiryKeyName, DateTime.UtcNow);
            return newItem;
        }

        try
        {
            if (!await _localStorage.ContainKeyAsync(keyName)) 
                return await UpdateCache();
            
            var lastUpdate = await _localStorage.GetItemAsync<DateTime>(expiryKeyName);
            if (lastUpdate + cacheDuration <= DateTime.UtcNow)
                return await UpdateCache();
            
            return await _localStorage.GetItemAsync<TCacheItem>(keyName);
        }
        catch
        {
            return await UpdateCache();
        }
    }
    
    public async Task<TGameState> CacheGameState<TGameState> (string key, Func<string, Task<TGameState?>> fetch)
        where TGameState : GameStateBase
    {
        var timeStampKey = $"{key}.LastUpdated";

        async Task<TGameState> RefreshGameState()
        {
            var state = await fetch(string.Empty);
            await _localStorage.SetItemAsync(key, state);
            await _localStorage.SetItemAsync(timeStampKey, DateTime.UtcNow);
            return state!;
        }

        if (!await _localStorage.ContainKeyAsync(key))
        {
            _logger.LogInformation("GameState is not present and will be fetched");
            return await RefreshGameState();
        }

        try
        {
            var cachedState = await _localStorage.GetItemAsync<TGameState>(key);
            var lastUpdated = await _localStorage.GetItemAsync<DateTime>(timeStampKey);

            _logger.LogDebug("GameState was cached {LastCache}", lastUpdated.ToLocalTime());

            // Only update every few hours
            if (DateTime.UtcNow - lastUpdated < TimeSpan.FromHours(GameStateUpdateFrequencyHours))
                return cachedState;

            try
            {
                var newState = await fetch(cachedState.Revision);

                if (newState == null || newState.Revision == cachedState.Revision)
                {
                    _logger.LogDebug("GameState is current up-to-date");
                    return cachedState;
                }

                _logger.LogInformation("GameState updated with new revision {OldRevision} to {NewRevision}",
                    cachedState.Revision, newState.Revision);
                await _localStorage.SetItemAsync(key, newState);
                await _localStorage.SetItemAsync(timeStampKey, DateTime.UtcNow);
                return newState;
            }

            // If update fails, log it but return the cached copy
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GameState failed to update, using cached version");
                return cachedState;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load GameState");
            return await RefreshGameState();
        }
    }
}