using Grpc.Core;
using Maple2.Database.Storage;
using Maple2.Server.Game.Session;
using Maple2.Server.Game.Util.Sync;
using Serilog;

namespace Maple2.Server.Game.Service;

public partial class ChannelService : Channel.Service.Channel.ChannelBase {
    private readonly GameServer server;
    private readonly PlayerInfoStorage playerInfos;
    private readonly GameStorage gameStorage;
    private readonly TableMetadataStorage tableMetadata;
    private readonly ServerTableMetadataStorage serverTableMetadata;

    private readonly ILogger logger = Log.Logger.ForContext<ChannelService>();

    public ChannelService(GameServer server, PlayerInfoStorage playerInfos, GameStorage gameStorage, ServerTableMetadataStorage serverTableMetadata, TableMetadataStorage tableMetadata, ItemMetadataStorage itemMetadata) {
        this.server = server;
        this.playerInfos = playerInfos;
        this.gameStorage = gameStorage;
        this.serverTableMetadata = serverTableMetadata;
        this.tableMetadata = tableMetadata;
    }

    // Enumerate sessions for character IDs, skipping those not connected.
    private void ForEachSession(IEnumerable<long> characterIds, Action<long, GameSession> action) {
        foreach (long characterId in characterIds) {
            if (!server.GetSession(characterId, out GameSession? session)) {
                continue;
            }
            action(characterId, session);
        }
    }

    // Require a connected session or throw a NotFound RPC error with a consistent message.
    private GameSession RequireSession(long characterId) {
        if (!server.GetSession(characterId, out GameSession? session)) {
            throw new RpcException(new Status(StatusCode.NotFound, $"Unable to find: {characterId}"));
        }
        return session;
    }
}
