using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.MwFifth;
using Larp.Landing.Shared;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class LandingService : ILandingService
{
    private readonly LarpContext _db;

    public LandingService(LarpContext db)
    {
        _db = db;
    }
    
    public async Task<Game[]> GetGames()
    {
        var games = await _db.Games.Find(x => true).ToListAsync();
        return games.ToArray();
    }

    public async Task<CharacterSummary[]> GetCharacters()
    {
        var characters = await _db.MwFifthGame.Characters.Find(_ => true).ToListAsync();
        return characters.Select(x => x.ToSummary()).ToArray();
    }
}

public class MwFifthGameService : IMwFifthGameService
{
    private readonly MwFifthGameContext _mwFifth;

    public MwFifthGameService(LarpContext larpContext)
    {
        _mwFifth = larpContext.MwFifthGame;
    }

    public async Task<GameState?> GetGameState(string lastRevision)
    {
        var state = await _mwFifth.GetGameState();
        return state.Revision == lastRevision ? null : state;
    }

    public async Task<Character?> GetCharacter(string characterId)
    {
        return await _mwFifth.Characters.Find(x => x.Id == characterId).FirstOrDefaultAsync();
    }
}