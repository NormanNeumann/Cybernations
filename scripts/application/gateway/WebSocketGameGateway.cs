using System;
using System.Collections.Generic;
using System.Text;
using Godot;

public sealed class WebSocketGameGateway : IGameGateway
{
    private readonly string _serverUrl;
    private readonly Queue<string> _pendingOutbound = new();
    private WebSocketPeer? _peer;
    private bool _initialized;

    public WebSocketGameGateway(string serverUrl)
    {
        _serverUrl = serverUrl.Trim();
    }

    public event Action<string>? ServerPacketReceived;

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        if (_serverUrl.Length == 0)
        {
            GD.PushWarning("WebSocketGameGateway: server URL is empty, gateway remains disconnected.");
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

        if (state == WebSocketPeer.State.Open)
        {
            FlushPendingMessages();
            ReceivePackets();
        }
    }

    public void SendPacket(string packetJson)
    {
        if (packetJson.Trim().Length == 0)
        {
            return;
        }

        QueueOrSend(packetJson);
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
            GD.PushWarning($"WebSocketGameGateway: connect failed ({error}).");
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

            var packetJson = Encoding.UTF8.GetString(packet);
            ServerPacketReceived?.Invoke(packetJson);
        }
    }
}
