using Grpc.Core;
using Larp.Data;
using Larp.Protos.Mystwood5e;

namespace Larp.WebService.GrpcServices;

public class Mystwood5eGrpcService : Larp.Protos.Mystwood5e.Mw5e.Mw5eBase
{
    private readonly LarpContext _larpContext;

    public Mystwood5eGrpcService(LarpContext larpContext)
    {
        _larpContext = larpContext;
    }

    public override async Task<GameStateResponse> GetGameState(UpdateCacheRequest request, ServerCallContext context)
    {
        var state = await _larpContext.FifthEdition.GetGameState();

        if (request.LastUpdated == null || request.LastUpdated != state.Revision)
            return new GameStateResponse() { GameState = state };

        return new GameStateResponse();
    }
}