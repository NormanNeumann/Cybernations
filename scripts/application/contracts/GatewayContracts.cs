using System;
using System.Collections.Generic;

public interface IGameGateway
{
    event Action<IReadOnlyList<ChatMessageVm>>? ChatLogSynced;

    void Initialize();
    void Poll();
    void SendChatMessage(string sender, string content);
    void NotifyPlayerDetailOpened(int slot);
    void Shutdown();
}
