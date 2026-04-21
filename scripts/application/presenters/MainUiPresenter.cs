using System;
using System.Collections.Generic;
using Godot;

public sealed class MainUiPresenter : IDisposable
{
    private const string LocalSenderName = "You";
    private const string LocalRoomId = "room-local";
    private const string LocalPlayerId = "client-local";

    private readonly IChatPanelView _chatPanelView;
    private readonly ITeamGoalPanelView _teamGoalPanelView;
    private readonly IInfoSummaryPanelView _infoSummaryPanelView;
    private readonly IHiveBoardView _hiveBoardView;
    private readonly IPlayerDetailPopupView _playerDetailPopupView;
    private readonly IGameGateway _gateway;
    private readonly Dictionary<int, Vector2> _pendingPlayerDetailPositions = [];

    private bool _teamGoalDetailOpenPending;
    private bool _infoSummaryDetailOpenPending;
    private bool _isBound;

    public MainUiPresenter(
        IChatPanelView chatPanelView,
        ITeamGoalPanelView teamGoalPanelView,
        IInfoSummaryPanelView infoSummaryPanelView,
        IHiveBoardView hiveBoardView,
        IPlayerDetailPopupView playerDetailPopupView,
        IGameGateway gateway
    )
    {
        _chatPanelView = chatPanelView;
        _teamGoalPanelView = teamGoalPanelView;
        _infoSummaryPanelView = infoSummaryPanelView;
        _hiveBoardView = hiveBoardView;
        _playerDetailPopupView = playerDetailPopupView;
        _gateway = gateway;
    }

    public void Initialize()
    {
        if (_isBound)
        {
            return;
        }

        _chatPanelView.ExpandRequested += OnChatExpandRequested;
        _chatPanelView.CollapseRequested += OnChatCollapseRequested;
        _chatPanelView.ChatSubmitted += OnChatSubmitted;

        _teamGoalPanelView.ToggleRequested += OnTeamGoalToggleRequested;
        _teamGoalPanelView.CloseRequested += OnTeamGoalCloseRequested;
        _infoSummaryPanelView.ToggleRequested += OnInfoSummaryToggleRequested;
        _infoSummaryPanelView.CloseRequested += OnInfoSummaryCloseRequested;

        _playerDetailPopupView.CloseRequested += OnPlayerDetailCloseRequested;
        _gateway.ServerPacketReceived += OnServerPacketReceived;

        _chatPanelView.SetExpanded(false);
        _teamGoalPanelView.SetDropdownVisible(false);
        _infoSummaryPanelView.SetDropdownVisible(false);
        _playerDetailPopupView.HidePopup();

        _gateway.Initialize();
        _gateway.SendPacket(
            GamePacketCodec.BuildCommand(
                PacketTypes.CmdSnapshotRequest,
                LocalRoomId,
                LocalPlayerId,
                new EmptyPayload()
            )
        );
        _isBound = true;
    }

    public void OnPlayerSelected(int slot, string progress, Vector2 preferredPosition)
    {
        _pendingPlayerDetailPositions[slot] = preferredPosition;
        _gateway.SendPacket(
            GamePacketCodec.BuildCommand(
                PacketTypes.CmdPlayerDetailRequest,
                LocalRoomId,
                LocalPlayerId,
                new PlayerDetailRequestPayload(
                    slot,
                    progress,
                    preferredPosition.X,
                    preferredPosition.Y
                )
            )
        );
    }

    public void Dispose()
    {
        if (!_isBound)
        {
            return;
        }

        _chatPanelView.ExpandRequested -= OnChatExpandRequested;
        _chatPanelView.CollapseRequested -= OnChatCollapseRequested;
        _chatPanelView.ChatSubmitted -= OnChatSubmitted;

        _teamGoalPanelView.ToggleRequested -= OnTeamGoalToggleRequested;
        _teamGoalPanelView.CloseRequested -= OnTeamGoalCloseRequested;
        _infoSummaryPanelView.ToggleRequested -= OnInfoSummaryToggleRequested;
        _infoSummaryPanelView.CloseRequested -= OnInfoSummaryCloseRequested;

        _playerDetailPopupView.CloseRequested -= OnPlayerDetailCloseRequested;
        _gateway.ServerPacketReceived -= OnServerPacketReceived;
        _isBound = false;
    }

    private void OnChatExpandRequested()
    {
        _chatPanelView.SetExpanded(true);
    }

    private void OnChatCollapseRequested()
    {
        _chatPanelView.SetExpanded(false);
    }

    private void OnChatSubmitted(string text)
    {
        _gateway.SendPacket(
            GamePacketCodec.BuildCommand(
                PacketTypes.CmdChatSubmit,
                LocalRoomId,
                LocalPlayerId,
                new ChatSubmitPayload(LocalSenderName, text)
            )
        );
    }

    private void OnTeamGoalToggleRequested()
    {
        if (_teamGoalPanelView.IsDropdownVisible)
        {
            _teamGoalDetailOpenPending = false;
            _teamGoalPanelView.SetDropdownVisible(false);
            return;
        }

        _teamGoalDetailOpenPending = true;
        _gateway.SendPacket(
            GamePacketCodec.BuildCommand(
                PacketTypes.CmdTeamGoalDetailRequest,
                LocalRoomId,
                LocalPlayerId,
                new EmptyPayload()
            )
        );
    }

    private void OnTeamGoalCloseRequested()
    {
        _teamGoalDetailOpenPending = false;
        _teamGoalPanelView.SetDropdownVisible(false);
    }

    private void OnInfoSummaryToggleRequested()
    {
        if (_infoSummaryPanelView.IsDropdownVisible)
        {
            _infoSummaryDetailOpenPending = false;
            _infoSummaryPanelView.SetDropdownVisible(false);
            return;
        }

        _infoSummaryDetailOpenPending = true;
        _gateway.SendPacket(
            GamePacketCodec.BuildCommand(
                PacketTypes.CmdInfoSummaryDetailRequest,
                LocalRoomId,
                LocalPlayerId,
                new EmptyPayload()
            )
        );
    }

    private void OnInfoSummaryCloseRequested()
    {
        _infoSummaryDetailOpenPending = false;
        _infoSummaryPanelView.SetDropdownVisible(false);
    }

    private void OnPlayerDetailCloseRequested()
    {
        _playerDetailPopupView.HidePopup();
    }

    private void OnServerPacketReceived(string packetJson)
    {
        if (!GamePacketCodec.TryParseEnvelope(packetJson, out var envelope))
        {
            GD.PushWarning($"MainUiPresenter: invalid server envelope '{packetJson}'.");
            return;
        }

        if (envelope.v != PacketTypes.Version)
        {
            GD.PushWarning($"MainUiPresenter: unsupported packet version '{envelope.v}'.");
            return;
        }

        switch (envelope.type)
        {
            case PacketTypes.EvtSnapshotFull:
                ApplySnapshotFull(envelope);
                break;
            case PacketTypes.EvtChatSync:
                ApplyChatSync(envelope);
                break;
            case PacketTypes.EvtPlayerDetail:
                ApplyPlayerDetail(envelope);
                break;
            case PacketTypes.EvtTeamGoalState:
                ApplyTeamGoalState(envelope);
                break;
            case PacketTypes.EvtInfoSummaryState:
                ApplyInfoSummaryState(envelope);
                break;
            case PacketTypes.EvtHiveBoardState:
                ApplyHiveBoardState(envelope);
                break;
            case PacketTypes.EvtError:
                ApplyError(envelope);
                break;
        }
    }

    private void ApplySnapshotFull(in PacketEnvelope envelope)
    {
        if (!GamePacketCodec.TryDeserializePayload<SnapshotFullPayload>(envelope, out var payload))
        {
            return;
        }

        _chatPanelView.SetMessages(payload.chat_messages ?? Array.Empty<ChatMessageVm>());
        if (payload.team_goal.HasValue)
        {
            var teamGoal = payload.team_goal.Value;
            _teamGoalPanelView.SetPreview(teamGoal.title, teamGoal.description);
        }

        if (payload.info_summary.HasValue)
        {
            var infoSummary = payload.info_summary.Value;
            _infoSummaryPanelView.SetSummary(infoSummary.title, infoSummary.body);
        }

        if (payload.hive_board.HasValue)
        {
            ApplyHiveBoardPayload(payload.hive_board.Value);
        }
    }

    private void ApplyChatSync(in PacketEnvelope envelope)
    {
        if (!GamePacketCodec.TryDeserializePayload<ChatSyncPayload>(envelope, out var payload))
        {
            return;
        }

        _chatPanelView.SetMessages(payload.messages ?? Array.Empty<ChatMessageVm>());
    }

    private void ApplyPlayerDetail(in PacketEnvelope envelope)
    {
        if (!GamePacketCodec.TryDeserializePayload<PlayerDetailPayload>(envelope, out var payload))
        {
            return;
        }

        var preferredPosition = new Vector2(payload.preferredX, payload.preferredY);
        if (preferredPosition == Vector2.Zero
            && _pendingPlayerDetailPositions.TryGetValue(payload.slot, out var pendingPosition))
        {
            preferredPosition = pendingPosition;
        }

        _pendingPlayerDetailPositions.Remove(payload.slot);

        _playerDetailPopupView.ShowPlayerDetail(
            new PlayerDetailVm(payload.slot, payload.progress, payload.description),
            preferredPosition
        );
    }

    private void ApplyTeamGoalState(in PacketEnvelope envelope)
    {
        if (!GamePacketCodec.TryDeserializePayload<TeamGoalStatePayload>(envelope, out var payload))
        {
            return;
        }

        _teamGoalPanelView.SetPreview(payload.title, payload.description);
        if (_teamGoalDetailOpenPending)
        {
            _teamGoalPanelView.SetDropdownVisible(true);
            _teamGoalDetailOpenPending = false;
        }
    }

    private void ApplyInfoSummaryState(in PacketEnvelope envelope)
    {
        if (!GamePacketCodec.TryDeserializePayload<InfoSummaryStatePayload>(envelope, out var payload))
        {
            return;
        }

        _infoSummaryPanelView.SetSummary(payload.title, payload.body);
        if (_infoSummaryDetailOpenPending)
        {
            _infoSummaryPanelView.SetDropdownVisible(true);
            _infoSummaryDetailOpenPending = false;
        }
    }

    private void ApplyHiveBoardState(in PacketEnvelope envelope)
    {
        if (!GamePacketCodec.TryDeserializePayload<HiveBoardStatePayload>(envelope, out var payload))
        {
            return;
        }

        ApplyHiveBoardPayload(payload);
    }

    private void ApplyHiveBoardPayload(HiveBoardStatePayload payload)
    {
        if (payload.tiles == null)
        {
            return;
        }

        var tiles = new List<BoardTileVm>(payload.tiles.Length);
        foreach (var tilePayload in payload.tiles)
        {
            if (!TryParseBoardTileKind(tilePayload.down, out var downKind))
            {
                continue;
            }

            BoardTileKind? upKind = null;
            if (!string.IsNullOrWhiteSpace(tilePayload.up)
                && TryParseBoardTileKind(tilePayload.up!, out var parsedUpKind))
            {
                upKind = parsedUpKind;
            }

            IReadOnlyList<BoardEdgeVm>? edges = null;
            if (tilePayload.edges != null)
            {
                var edgeList = new List<BoardEdgeVm>(tilePayload.edges.Length);
                foreach (var edgePayload in tilePayload.edges)
                {
                    edgeList.Add(
                        new BoardEdgeVm(
                            edgePayload.edge,
                            edgePayload.relation_texture_path,
                            ParseBoardPathKind(edgePayload.path_kind),
                            edgePayload.rotation_steps,
                            edgePayload.path_texture_path
                        )
                    );
                }

                edges = edgeList;
            }

            tiles.Add(
                new BoardTileVm(
                    tilePayload.index,
                    downKind,
                    upKind,
                    tilePayload.conflict,
                    edges
                )
            );
        }

        _hiveBoardView.ApplyTiles(tiles);
    }

    private static bool TryParseBoardTileKind(string? value, out BoardTileKind kind)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            kind = BoardTileKind.Wilds;
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant().Replace("_", string.Empty).Replace("-", string.Empty);
        switch (normalized)
        {
            case "wilds":
            case "wild":
                kind = BoardTileKind.Wilds;
                return true;
            case "wasted":
                kind = BoardTileKind.Wasted;
                return true;
            case "human":
                kind = BoardTileKind.Human;
                return true;
            case "technology":
            case "tech":
                kind = BoardTileKind.Technology;
                return true;
            default:
                kind = BoardTileKind.Wilds;
                return false;
        }
    }

    private static BoardPathKind ParseBoardPathKind(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return BoardPathKind.None;
        }

        var normalized = value.Trim().ToLowerInvariant().Replace("_", string.Empty).Replace("-", string.Empty);
        return normalized switch
        {
            "none" => BoardPathKind.None,
            "typea" => BoardPathKind.TypeA,
            "typeb" => BoardPathKind.TypeB,
            "typec" => BoardPathKind.TypeC,
            "typed" => BoardPathKind.TypeD,
            "typee" => BoardPathKind.TypeE,
            _ => BoardPathKind.None,
        };
    }

    private static void ApplyError(in PacketEnvelope envelope)
    {
        if (!GamePacketCodec.TryDeserializePayload<ErrorPayload>(envelope, out var payload))
        {
            GD.PushWarning($"MainUiPresenter: server error event without payload, req_id={envelope.req_id ?? "none"}");
            return;
        }

        GD.PushWarning($"MainUiPresenter: server error code={payload.code}, reason={payload.reason}, req_id={envelope.req_id ?? "none"}");
    }
}
