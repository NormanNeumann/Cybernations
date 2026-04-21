using System;
using System.Collections.Generic;

public sealed class LoopbackGameGateway : IGameGateway
{
    private readonly List<ChatMessageVm> _chatMessages =
    [
        new ChatMessageVm("P1", "Secure the wilds."),
        new ChatMessageVm("P3", "Human build next turn."),
        new ChatMessageVm("P5", "Conflict blocks the final cells."),
    ];

    public event Action<IReadOnlyList<ChatMessageVm>>? ChatLogSynced;

    public void Initialize()
    {
        PublishChatLog();
    }

    public void Poll()
    {
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
    }

    public void NotifyPlayerDetailOpened(int slot)
    {
        // Placeholder for future packet sending: inspect/open player detail on backend.
    }

    public void Shutdown()
    {
    }

    private void PublishChatLog()
    {
        ChatLogSynced?.Invoke(_chatMessages.AsReadOnly());
    }
}
