using System.Text.Json;

public readonly record struct PacketEnvelope(
    int v,
    string type,
    string msg_id,
    string? req_id,
    long? seq,
    string room_id,
    string player_id,
    long client_ts,
    JsonElement payload
);

public readonly record struct EmptyPayload();

public readonly record struct ChatSubmitPayload(string sender, string content);

public readonly record struct ChatSyncPayload(ChatMessageVm[] messages);

public readonly record struct TeamGoalStatePayload(string title, string description);

public readonly record struct InfoSummaryStatePayload(string title, string body);

public readonly record struct HiveBoardEdgePayload(
    int edge,
    string? relation_texture_path,
    string path_kind,
    int rotation_steps,
    string? path_texture_path
);

public readonly record struct HiveBoardTilePayload(
    int index,
    string down,
    string? up,
    bool conflict,
    HiveBoardEdgePayload[]? edges
);

public readonly record struct HiveBoardStatePayload(HiveBoardTilePayload[] tiles);

public readonly record struct SnapshotFullPayload(
    ChatMessageVm[] chat_messages,
    TeamGoalStatePayload? team_goal,
    InfoSummaryStatePayload? info_summary,
    HiveBoardStatePayload? hive_board
);

public readonly record struct PlayerDetailRequestPayload(int slot, string progress, float preferredX, float preferredY);

public readonly record struct PlayerDetailPayload(int slot, string progress, string description, float preferredX, float preferredY);

public readonly record struct ErrorPayload(string code, string reason);
