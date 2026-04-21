using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot;

public sealed class WebSocketGameGateway : IGameGateway
{
    private readonly string _serverUrl;
    private readonly List<ChatMessageVm> _chatMessages =
    [
        new ChatMessageVm("P1", "Secure the wilds."),
        new ChatMessageVm("P3", "Human build next turn."),
        new ChatMessageVm("P5", "Conflict blocks the final cells."),
    ];

    private readonly Queue<string> _pendingOutbound = new();
    private WebSocketPeer? _peer;
    private bool _initialized;
    private bool _syncRequestedAfterOpen;
    private WebSocketPeer.State _lastState = WebSocketPeer.State.Closed;

    public WebSocketGameGateway(string serverUrl)
    {
        _serverUrl = serverUrl.Trim();
    }

    public event Action<IReadOnlyList<ChatMessageVm>>? ChatLogSynced;

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        PublishChatLog();

        if (_serverUrl.Length == 0)
        {
            GD.Print("WebSocketGameGateway: server URL is empty, running in local loopback mode.");
            return;
        }

        Connect();
    }

    public void Poll()
    {
        if (!_initialized || _peer == null)
        {
            return;
        }

        _peer.Poll();
        var state = _peer.GetReadyState();

        if (state == WebSocketPeer.State.Open && _lastState != WebSocketPeer.State.Open)
        {
            _syncRequestedAfterOpen = false;
        }

        if (state == WebSocketPeer.State.Open)
        {
            RequestChatSyncIfNeeded();
            FlushPendingMessages();
            ReceivePackets();
        }

        _lastState = state;
    }

    public void SendChatMessage(string sender, string content)
    {
        var trimmed = content.Trim();
        if (trimmed.Length == 0)
        {
            return;
        }

        _chatMessages.Add(new ChatMessageVm(sender, trimmed));
        PublishChatLog();

        var packet = JsonSerializer.Serialize(new ChatSubmitPacket("chat_submit", sender, trimmed));
        QueueOrSend(packet);
    }

    public void NotifyPlayerDetailOpened(int slot)
    {
        var packet = JsonSerializer.Serialize(new PlayerDetailOpenedPacket("player_detail_opened", slot));
        QueueOrSend(packet);
    }

    public void Shutdown()
    {
        if (_peer == null)
        {
            return;
        }

        if (_peer.GetReadyState() != WebSocketPeer.State.Closed)
        {
            _peer.Close();
        }

        _peer = null;
        _pendingOutbound.Clear();
        _initialized = false;
    }

    private void Connect()
    {
        _peer = new WebSocketPeer();
        var error = _peer.ConnectToUrl(_serverUrl);
        if (error != Error.Ok)
        {
            GD.PushWarning($"WebSocketGameGateway: connect failed ({error}), fallback to local mode.");
            _peer = null;
        }
    }

    private void QueueOrSend(string packet)
    {
        if (_peer == null || _peer.GetReadyState() != WebSocketPeer.State.Open)
        {
            _pendingOutbound.Enqueue(packet);
            return;
        }

        SendRaw(packet);
    }

    private void FlushPendingMessages()
    {
        while (_pendingOutbound.Count > 0 && _peer != null && _peer.GetReadyState() == WebSocketPeer.State.Open)
        {
            SendRaw(_pendingOutbound.Dequeue());
        }
    }

    private void SendRaw(string packet)
    {
        if (_peer == null)
        {
            return;
        }

        var error = _peer.Send(Encoding.UTF8.GetBytes(packet));
        if (error != Error.Ok)
        {
            GD.PushWarning($"WebSocketGameGateway: send failed ({error}).");
        }
    }

    private void RequestChatSyncIfNeeded()
    {
        if (_syncRequestedAfterOpen)
        {
            return;
        }

        var packet = JsonSerializer.Serialize(new SyncRequestPacket("chat_sync_request"));
        SendRaw(packet);
        _syncRequestedAfterOpen = true;
    }

    private void ReceivePackets()
    {
        if (_peer == null)
        {
            return;
        }

        while (_peer.GetAvailablePacketCount() > 0)
        {
            var packet = _peer.GetPacket();
            if (packet.Length == 0)
            {
                continue;
            }

            var text = Encoding.UTF8.GetString(packet);
            HandleInboundPacket(text);
        }
    }

    private void HandleInboundPacket(string packetText)
    {
        try
        {
            using var document = JsonDocument.Parse(packetText);
            if (!document.RootElement.TryGetProperty("type", out var typeElement))
            {
                return;
            }

            var type = typeElement.GetString();
            if (type == "chat_sync")
            {
                ApplyChatSync(document.RootElement);
                return;
            }

            if (type == "chat_message")
            {
                AppendChatMessage(document.RootElement);
            }
        }
        catch (Exception exception)
        {
            GD.PushWarning($"WebSocketGameGateway: invalid packet '{packetText}', error: {exception.Message}");
        }
    }

    private void ApplyChatSync(JsonElement payload)
    {
        if (!payload.TryGetProperty("messages", out var messagesElement) || messagesElement.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        _chatMessages.Clear();
        foreach (var item in messagesElement.EnumerateArray())
        {
            var sender = item.TryGetProperty("sender", out var senderElement)
                ? senderElement.GetString() ?? "System"
                : "System";
            var content = item.TryGetProperty("content", out var contentElement)
                ? contentElement.GetString() ?? string.Empty
                : string.Empty;

            _chatMessages.Add(new ChatMessageVm(sender, content));
        }

        PublishChatLog();
    }

    private void AppendChatMessage(JsonElement payload)
    {
        if (!payload.TryGetProperty("sender", out var senderElement)
            || !payload.TryGetProperty("content", out var contentElement))
        {
            return;
        }

        var sender = senderElement.GetString() ?? "System";
        var content = contentElement.GetString() ?? string.Empty;
        _chatMessages.Add(new ChatMessageVm(sender, content));
        PublishChatLog();
    }

    private void PublishChatLog()
    {
        ChatLogSynced?.Invoke(_chatMessages.AsReadOnly());
    }

    private readonly record struct ChatSubmitPacket(string type, string sender, string content);
    private readonly record struct PlayerDetailOpenedPacket(string type, int slot);
    private readonly record struct SyncRequestPacket(string type);
}
