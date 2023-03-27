using System.Threading.Channels;
using Larp.Data;
using Larp.Data.Mongo;
using Larp.Data.Mongo.Services;
using Larp.Landing.Shared;
using Larp.Landing.Shared.Messages;
using MongoDB.Driver;

namespace Larp.Landing.Server.Services;

public class LandingServiceServer : ILandingService
{
    private readonly LarpContext _db;
    private readonly IUserSessionManager _userSessionManager;

    public LandingServiceServer(LarpContext db, IUserSessionManager userSessionManager)
    {
        _db = db;
        _userSessionManager = userSessionManager;
    }

    public async Task<Result> Login(string email, string origin)
    {
        await _userSessionManager.GenerateToken(email, origin);
        return Result.Success;
    }

    public async Task<StringResult> Confirm(string email, string token)
    {
        var sessionId = await _userSessionManager.CreateUserSession(email, token);
        return StringResult.Success(sessionId);
    }

    public async Task<Result> Validate(string token)
    {
        var result = await _userSessionManager.ValidateUserSession(token);
        switch (result.StatusCode)
        {
            case UserSessionValidationResultStatusCode.Authenticated: 
                return Result.Success;
            case UserSessionValidationResultStatusCode.Invalid:
            case UserSessionValidationResultStatusCode.NotConfirmed:
            case UserSessionValidationResultStatusCode.Expired:
            default:
                return Result.Failed(result.StatusCode.ToString());
        }
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