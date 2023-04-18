using Larp.Data.MwFifth;
using Larp.Landing.Shared.MwFifth;

namespace Larp.Landing.Server.Services;

public class MwFifthServiceServer : IMwFifthService
{
    private readonly MwFifthCharacterManager _manager;
    private readonly Account _account;
    private readonly MwFifthGameContext _mwFifth;

    public MwFifthServiceServer(MwFifthCharacterManager manager, IUserSession userSession, LarpContext larpContext)
    {
        _manager = manager;
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
}