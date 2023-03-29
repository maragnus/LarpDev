using System.Collections.Concurrent;
using Blazored.LocalStorage;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared.MwFifth;

namespace Larp.Landing.Client.Services;

public class MwFifthService : IAsyncDisposable
{
    private readonly IMwFifthService _mwFifth;
    private readonly ILocalStorageService _localStorage;
    private readonly DataCacheService _dataCache;
    private readonly LandingService _landingService;

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

    public async Task<Character> GetCharacter(string characterId) =>
        await _mwFifth.GetCharacter(characterId);

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
        var id = await _mwFifth.SaveCharacter(character);
        if (id.IsSuccess)
            character.Id = id.Value!;
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

    public async Task DeleteCharacter(string characterId) =>
        await _mwFifth.DeleteCharacter(characterId);
}