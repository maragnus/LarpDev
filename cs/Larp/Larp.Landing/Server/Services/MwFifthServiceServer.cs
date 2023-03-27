using Larp.Data.Mongo;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class MwFifthServiceServer : IMwFifthService
{
    private readonly IUserManager _userManager;
    private readonly MwFifthGameContext _mwFifth;

    public MwFifthServiceServer(LarpContext larpContext, IUserManager userManager)
    {
        _userManager = userManager;
        _mwFifth = larpContext.MwFifthGame;
    }

    public async Task<GameState?> GetGameState(string lastRevision)
    {
        var state = await _mwFifth.GetGameState();
        return state.Revision == lastRevision 
            ? new GameState() { Revision = state.Revision } 
            : state;
    }

    public async Task<Character?> GetCharacter(string characterId)
    {
        return await _mwFifth.Characters.Find(x => x.Id == characterId).FirstOrDefaultAsync();
    }

    public async Task<Character> GetNewCharacter()
    {
        return new Character()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AccountId = _userManager.CurrentUser.AccountId 
        };
    }

    public async Task<bool> SaveCharacter(Character character)
    {
        throw new NotImplementedException();
    }
}