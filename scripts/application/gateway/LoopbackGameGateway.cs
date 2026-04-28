using System;
using System.Collections.Generic;
using Godot;

public sealed class LoopbackGameGateway : IGameGateway
{
	private const string ServerPlayerId = "server";

	private readonly List<ChatMessageVm> _chatMessages =
	[
		new ChatMessageVm("P1", "Secure the wilds."),
		new ChatMessageVm("P3", "Human build next turn."),
		new ChatMessageVm("P5", "Conflict blocks the final cells."),
	];
	private readonly TeamGoalStatePayload _teamGoalState =
		new("Team Goal", "Shared objective for every player:\nStabilize the board and keep conflict under control.");
	private readonly InfoSummaryStatePayload _infoSummaryState =
		new("Information Panel", "Key state summary:\n- 5 players on the left\n- 11 map hexes in the center\n- 3 resource tracks at the bottom");
	private readonly HiveBoardTilePayload[] _hiveBoardTiles =
	[
		new HiveBoardTilePayload(0, "wilds", null, false, null),
		new HiveBoardTilePayload(1, "wilds", null, false, null),
		new HiveBoardTilePayload(2, "wilds", null, false, null),
		new HiveBoardTilePayload(3, "wasted", null, false, null),
		new HiveBoardTilePayload(4, "wilds", null, false, null),
		new HiveBoardTilePayload(5, "wilds", "human", false, null),
		new HiveBoardTilePayload(6, "wasted", "human", false, null),
		new HiveBoardTilePayload(7, "wilds", null, false, null),
		new HiveBoardTilePayload(8, "wilds", null, false, null),
		new HiveBoardTilePayload(9, "wasted", "technology", false, null),
		new HiveBoardTilePayload(10, "wasted", "technology", false, null),
	];
	private long _nextSequence = 1;

	public event Action<string>? ServerPacketReceived;

	public void Initialize()
	{
	}

	public void Poll()
	{
	}

	public void SendPacket(string packetJson)
	{
		if (!GamePacketCodec.TryParseEnvelope(packetJson, out var envelope))
		{
			GD.PushWarning($"LoopbackGameGateway: invalid command envelope '{packetJson}'.");
			return;
		}

		if (envelope.v != PacketTypes.Version)
		{
			EmitError(envelope, "unsupported_version", $"Version {envelope.v} is not supported.");
			return;
		}

		switch (envelope.type)
		{
			case PacketTypes.CmdSnapshotRequest:
				EmitSnapshotFull(envelope);
				break;
			case PacketTypes.CmdChatSubmit:
				HandleChatSubmit(envelope);
				break;
			case PacketTypes.CmdPlayerDetailRequest:
				EmitPlayerDetail(envelope);
				break;
			case PacketTypes.CmdTeamGoalDetailRequest:
				EmitTeamGoalState(envelope);
				break;
			case PacketTypes.CmdInfoSummaryDetailRequest:
				EmitInfoSummaryState(envelope);
				break;
			default:
				EmitError(envelope, "unsupported_command", $"Unsupported command '{envelope.type}'.");
				break;
		}
	}

	public void Shutdown()
	{
	}

	private void HandleChatSubmit(in PacketEnvelope envelope)
	{
		if (!GamePacketCodec.TryDeserializePayload<ChatSubmitPayload>(envelope, out var payload))
		{
			EmitError(envelope, "invalid_payload", "chat_submit payload is invalid.");
			return;
		}

		var trimmed = payload.content.Trim();
		if (trimmed.Length == 0)
		{
			EmitError(envelope, "empty_chat", "Chat content cannot be empty.");
			return;
		}

		_chatMessages.Add(new ChatMessageVm(payload.sender, trimmed));
		EmitChatSync(envelope);
	}

	private void EmitPlayerDetail(in PacketEnvelope envelope)
	{
		if (!GamePacketCodec.TryDeserializePayload<PlayerDetailRequestPayload>(envelope, out var payload))
		{
			EmitError(envelope, "invalid_payload", "player_detail_request payload is invalid.");
			return;
		}

		var detailPayload = new PlayerDetailPayload(
			payload.slot,
			payload.progress,
			"More player data can be rendered here.",
			payload.preferredX,
			payload.preferredY
		);
		EmitEvent(PacketTypes.EvtPlayerDetail, envelope, detailPayload);
	}

	private void EmitSnapshotFull(in PacketEnvelope envelope)
	{
		EmitEvent(
			PacketTypes.EvtSnapshotFull,
			envelope,
			new SnapshotFullPayload(
				_chatMessages.ToArray(),
				_teamGoalState,
				_infoSummaryState,
				new HiveBoardStatePayload(_hiveBoardTiles)
			)
		);
	}

	private void EmitChatSync(in PacketEnvelope envelope)
	{
		EmitEvent(
			PacketTypes.EvtChatSync,
			envelope,
			new ChatSyncPayload(_chatMessages.ToArray())
		);
	}

	private void EmitTeamGoalState(in PacketEnvelope envelope)
	{
		EmitEvent(PacketTypes.EvtTeamGoalState, envelope, _teamGoalState);
	}

	private void EmitInfoSummaryState(in PacketEnvelope envelope)
	{
		EmitEvent(PacketTypes.EvtInfoSummaryState, envelope, _infoSummaryState);
	}

	private void EmitError(in PacketEnvelope envelope, string code, string reason)
	{
		EmitEvent(PacketTypes.EvtError, envelope, new ErrorPayload(code, reason));
	}

	private void EmitEvent<TPayload>(string eventType, in PacketEnvelope requestEnvelope, TPayload payload)
	{
		var packet = GamePacketCodec.BuildEvent(
			eventType,
			requestEnvelope.room_id,
			ServerPlayerId,
			payload,
			NextSequence(),
			requestEnvelope.msg_id
		);
		ServerPacketReceived?.Invoke(packet);
	}

	private long NextSequence()
	{
		return _nextSequence++;
	}
}
