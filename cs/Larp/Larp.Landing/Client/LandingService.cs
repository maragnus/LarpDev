using System.Collections.Concurrent;
using Blazored.LocalStorage;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using MongoDB.Bson;

namespace Larp.Landing.Client;

public enum AutoSaveState
{
    Inactive,
    ChangeAvailable,
    Saved,
    Saving
}

public class MwFifthService : IAsyncDisposable
{
    private readonly IMwFifthService _mwFifth;
    private readonly ILocalStorageService _localStorage;
    private readonly DataCacheService _dataCache;
    private readonly LandingService _landingService;

    private const string DraftCharacterStorageKey = "MwFifth.NewCharacter";

    public AutoSaveState AutoSaveState { get; private set; } = AutoSaveState.Inactive;
    public EventHandler? AutoSaveStateChange;
    
    public MwFifthService(LandingService landingService, IMwFifthService mwFifth, ILocalStorageService localStorage,
        DataCacheService dataCache)
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

    public async Task<Character> GetDraftCharacter()
    {
        return await _mwFifth.GetNewCharacter();
    }

    public async Task Save(Character character)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    private ConcurrentDictionary<string, Character>? _autoSaves;
    private Task? _autoSavingTask;
    private CancellationTokenSource? _autoSaveFinished;
    
    public void StartAutoSaving()
    {
        if (_autoSavingTask != null) return;
        _autoSaves ??= new();
        _autoSaveFinished = new CancellationTokenSource();

        var token = _autoSaveFinished.Token;
        _autoSavingTask = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await SaveAutoSaves();
                    await Task.Delay(TimeSpan.FromSeconds(15), token);
                }
                catch
                {
                    // ignored
                }
            }
        }, CancellationToken.None);
    }

    private async Task SaveAutoSaves()
    {
        if (_autoSaves == null) return;
        
        AutoSaveState = AutoSaveState.Saving;
        AutoSaveStateChange?.Invoke(this, EventArgs.Empty);
        
        foreach (var key in _autoSaves.Keys.ToList())
        {
            if (_autoSaves.TryRemove(key, out var character))
            {
                await Save(character);
            }
        }

        AutoSaveState = AutoSaveState.Saved;
        AutoSaveStateChange?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateAutoSave(Character character)
    {
        StartAutoSaving();
        _autoSaves!.TryAdd(character.Id, character);
        AutoSaveState = AutoSaveState.ChangeAvailable;
        AutoSaveStateChange?.Invoke(this, EventArgs.Empty);
    }

    public async Task FinishAutoSave(Character character)
    {
        if (_autoSaves?.TryRemove(character.Id, out var c) == true)
            await Save(c);
    }
    
    public async Task FinishAutoSave(TimeSpan? timeOut)
    {
        if (_autoSavingTask != null)
        {
            _autoSaveFinished?.Cancel();
            await _autoSavingTask.WaitAsync(timeOut ?? TimeSpan.FromSeconds(15));
        }
        await SaveAutoSaves();
    }

    public async ValueTask DisposeAsync()
    {
        await FinishAutoSave(TimeSpan.FromSeconds(5));
    }
}

public class LandingService
{
    private readonly ILandingService _landing;
    private readonly DataCacheService _dataCache;
    private readonly ILogger<LandingService> _logger;

    private const int GameStateUpdateFrequencyHours = 4;

    public LandingService(ILandingService landing,
        IMwFifthService mwFifth,
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