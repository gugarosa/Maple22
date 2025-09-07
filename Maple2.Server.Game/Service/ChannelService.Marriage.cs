using Grpc.Core;
using Maple2.Server.Game.Session;

namespace Maple2.Server.Game.Service;

public partial class ChannelService {
    public override Task<MarriageResponse> Marriage(MarriageRequest request, ServerCallContext context) {
        GameSession session = RequireSession(request.ReceiverId);

        switch (request.MarriageCase) {
            case MarriageRequest.MarriageOneofCase.RemoveMarriage:
                session.Marriage.RemoveMarriage();
                break;
            case MarriageRequest.MarriageOneofCase.RemoveWeddingHall:
                session.Marriage.RemoveWeddingHall();
                break;
        }

        return Task.FromResult(new MarriageResponse());
    }
}
