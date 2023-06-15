using Larp.Data.MwFifth;
using Larp.Landing.Shared.MwFifth;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class MwFifthServiceServer : IMwFifthService
{
    private readonly Account _account;
    private readonly LarpContext _larpContext;
    private readonly MwFifthCharacterManager _manager;
    private readonly MwFifthGameContext _mwFifth;

    public MwFifthServiceServer(MwFifthCharacterManager manager, IUserSession userSession, LarpContext larpContext)
    {
        _manager = manager;
        _larpContext = larpContext;
        _mwFifth = larpContext.MwFifthGame;
        _account = userSession.Account!;
    }

    public async Task<GameState> GetGameState(string lastRevision)
    {
        var state = await _mwFifth.GetGameState();
        return state.Revision == lastRevision
            ? new GameState { Revision = state.Revision }
            : state;
    }

    public async Task<CharacterAndRevision> GetCharacter(string characterId) =>
        await _manager.GetRevision(characterId, _account, false);

    public async Task<CharacterAndRevision> ReviseCharacter(string characterId) =>
        await _manager.GetDraft(characterId, _account, false);

    public async Task<CharacterAndRevision> GetNewCharacter() =>
        await _manager.GetNew(_account);

    public async Task SaveCharacter(CharacterRevision revision) =>
        await _manager.Save(revision, _account, false);

    public async Task DeleteCharacter(string characterId) =>
        await _manager.Delete(characterId, _account, false);

    public async Task<ClarifyTerm[]> GetTerms()
    {
        var gameId = await _larpContext.GetGameIdByName(GameState.GameName);
        return (await _larpContext.ClarifyTerms
                .Find(term => term.GameId == gameId)
                .ToListAsync())
            .ToArray();
    }
}