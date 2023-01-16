using JetBrains.Annotations;
using Larp.Data;
using Larp.Protos.Mystwood5e;
using Larp.WebService.ProtobufControllers;

namespace Larp.WebService.Controllers;

[PublicAPI]
// ReSharper disable once InconsistentNaming
public class Mw5eController : ProtobufController
{
    private LarpContext _larpContext;

    public Mw5eController(LarpContext larpContext)
    {
        _larpContext = larpContext;
    }

    public async Task<GameStateResponse> GetGameState(UpdateCacheRequest request)
    {
        var state = await _larpContext.FifthEdition.GetGameState();

        if (request.LastUpdated == null || request.LastUpdated != state.Revision)
            return new GameStateResponse() { GameState = state };

        return new GameStateResponse();
    }
}