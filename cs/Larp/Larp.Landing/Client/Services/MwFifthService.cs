using System.Collections.Concurrent;
using Blazored.LocalStorage;
using Larp.Data;
using Larp.Data.MwFifth;
using Larp.Landing.Shared.MwFifth;

namespace Larp.Landing.Client.Services;

public class MwFifthService
{
    private readonly IMwFifthService _mwFifth;
    private readonly DataCacheService _dataCache;
    private readonly LandingService _landingService;

    public MwFifthService(LandingService landingService, IMwFifthService mwFifth, DataCacheService dataCache)
    {
        _mwFifth = mwFifth;
        _dataCache = dataCache;
        _landingService = landingService;
    }

    public async Task<CharacterAndRevision> GetCharacter(string characterId) =>
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

    public async Task<CharacterAndRevision> GetDraftCharacter() =>
        await _mwFifth.GetNewCharacter();

    public async Task Save(CharacterRevision revision) =>
        await _mwFifth.SaveCharacter(revision);

    public async Task DeleteCharacter(string characterId) =>
        await _mwFifth.DeleteCharacter(characterId);

    public async Task<CharacterAndRevision> StartDraft(string characterId) =>
        await _mwFifth.ReviseCharacter(characterId);
}