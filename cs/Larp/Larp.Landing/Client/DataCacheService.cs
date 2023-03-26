using Blazored.LocalStorage;
using Larp.Data.MwFifth;

namespace Larp.Landing.Client;

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
        var expiryKeyName = $"{keyName}.Expires";

        async Task<TCacheItem> UpdateCache()
        {
            var newItem = await updateCache();
            await _localStorage.SetItemAsync(keyName, newItem);
            await _localStorage.SetItemAsync(expiryKeyName, DateTime.UtcNow + cacheDuration);
            return newItem;
        }

        try
        {
            if (!await _localStorage.ContainKeyAsync(keyName)) 
                return await UpdateCache();
            
            var expires = await _localStorage.GetItemAsync<DateTime>(expiryKeyName);
            if (expires <= DateTime.UtcNow)
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

            if (newState == null || newState.Revision == cachedState.Revision)
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