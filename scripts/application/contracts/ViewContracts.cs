using System;
using System.Collections.Generic;
using Godot;

public readonly record struct ChatMessageVm(string Sender, string Content);

public readonly record struct PlayerDetailVm(int Slot, string Progress, string Description);

public interface IChatPanelView
{
    event Action? ExpandRequested;
    event Action? CollapseRequested;
    event Action<string>? ChatSubmitted;

    bool IsExpanded { get; }
    void SetExpanded(bool expanded);
    void SetMessages(IReadOnlyList<ChatMessageVm> messages);
}

public interface ITeamGoalPanelView
{
    event Action? ToggleRequested;
    event Action? CloseRequested;

    bool IsDropdownVisible { get; }
    void SetDropdownVisible(bool visible);
}

public interface IPlayerDetailPopupView
{
    event Action? CloseRequested;

    bool IsOpen { get; }
    void ShowPlayerDetail(PlayerDetailVm detail, Vector2 preferredPosition);
    void HidePopup();
}
