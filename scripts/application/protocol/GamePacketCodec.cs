using System;
using System.Text.Json;

public static class GamePacketCodec
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static string BuildCommand<TPayload>(
        string type,
        string roomId,
        string playerId,
        TPayload payload,
        string? reqId = null
    )
    {
        return BuildEnvelope(
            type,
            roomId,
            playerId,
            payload,
            reqId,
            sequence: null
        );
    }

    public static string BuildEvent<TPayload>(
        string type,
        string roomId,
        string playerId,
        TPayload payload,
        long sequence,
        string? reqId = null
    )
    {
        return BuildEnvelope(
            type,
            roomId,
            playerId,
            payload,
            reqId,
            sequence
        );
    }

    public static bool TryParseEnvelope(string packetJson, out PacketEnvelope envelope)
    {
        envelope = default;
        if (packetJson.Trim().Length == 0)
        {
            return false;
        }

        try
        {
            var dto = JsonSerializer.Deserialize<PacketEnvelopeDto>(packetJson, JsonOptions);
            if (dto == null
                || dto.v <= 0
                || string.IsNullOrWhiteSpace(dto.type)
                || string.IsNullOrWhiteSpace(dto.msg_id)
                || string.IsNullOrWhiteSpace(dto.room_id)
                || string.IsNullOrWhiteSpace(dto.player_id)
                || dto.client_ts <= 0)
            {
                return false;
            }

            var payload = dto.payload.ValueKind == JsonValueKind.Undefined
                ? JsonDocument.Parse("{}").RootElement.Clone()
                : dto.payload.Clone();

            envelope = new PacketEnvelope(
                dto.v,
                dto.type!,
                dto.msg_id!,
                dto.req_id,
                dto.seq,
                dto.room_id!,
                dto.player_id!,
                dto.client_ts,
                payload
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryDeserializePayload<TPayload>(in PacketEnvelope envelope, out TPayload payload)
    {
        try
        {
            var parsed = envelope.payload.Deserialize<TPayload>(JsonOptions);
            if (parsed == null)
            {
                payload = default!;
                return false;
            }

            payload = parsed;
            return true;
        }
        catch
        {
            payload = default!;
            return false;
        }
    }

    private static string BuildEnvelope<TPayload>(
        string type,
        string roomId,
        string playerId,
        TPayload payload,
        string? reqId,
        long? sequence
    )
    {
        var dto = new PacketEnvelopeDto
        {
            v = PacketTypes.Version,
            type = type,
            msg_id = Guid.NewGuid().ToString("N"),
            req_id = reqId,
            seq = sequence,
            room_id = roomId,
            player_id = playerId,
            client_ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            payload = JsonSerializer.SerializeToElement(payload, JsonOptions),
        };
        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    private sealed class PacketEnvelopeDto
    {
        public int v { get; set; }
        public string? type { get; set; }
        public string? msg_id { get; set; }
        public string? req_id { get; set; }
        public long? seq { get; set; }
        public string? room_id { get; set; }
        public string? player_id { get; set; }
        public long client_ts { get; set; }
        public JsonElement payload { get; set; }
    }
}
