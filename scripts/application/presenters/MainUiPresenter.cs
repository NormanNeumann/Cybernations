using System;
using System.Collections.Generic;
using Godot;

public sealed class MainUiPresenter : IDisposable
{
    private const string LocalSenderName = "You";

    private readonly IChatPanelView _chatPanelView;
    private readonly ITeamGoalPanelView _teamGoalPanelView;
    private readonly IPlayerDetailPopupView _playerDetailPopupView;
    private readonly IGameGateway _gateway;

    private bool _isBound;

    public MainUiPresenter(
        IChatPanelView chatPanelView,
        ITeamGoalPanelView teamGoalPanelView,
        IPlayerDetailPopupView playerDetailPopupView,
        IGameGateway gateway
    )
    {
        _chatPanelView = chatPanelView;
        _teamGoalPanelView = teamGoalPanelView;
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

        _playerDetailPopupView.CloseRequested += OnPlayerDetailCloseRequested;
        _gateway.ChatLogSynced += OnChatLogSynced;

        _chatPanelView.SetExpanded(false);
        _teamGoalPanelView.SetDropdownVisible(false);
        _playerDetailPopupView.HidePopup();

        _gateway.Initialize();
        _isBound = true;
    }

    public void OnPlayerSelected(int slot, string progress, Vector2 preferredPosition)
    {
        var detail = new PlayerDetailVm(
            slot,
            progress,
            "More player data can be rendered here."
        );

        _playerDetailPopupView.ShowPlayerDetail(detail, preferredPosition);
        _gateway.NotifyPlayerDetailOpened(slot);
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

        _playerDetailPopupView.CloseRequested -= OnPlayerDetailCloseRequested;
        _gateway.ChatLogSynced -= OnChatLogSynced;
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
        _gateway.SendChatMessage(LocalSenderName, text);
    }

    private void OnTeamGoalToggleRequested()
    {
        _teamGoalPanelView.SetDropdownVisible(!_teamGoalPanelView.IsDropdownVisible);
    }

    private void OnTeamGoalCloseRequested()
    {
        _teamGoalPanelView.SetDropdownVisible(false);
    }

    private void OnPlayerDetailCloseRequested()
    {
        _playerDetailPopupView.HidePopup();
    }

    private void OnChatLogSynced(IReadOnlyList<ChatMessageVm> messages)
    {
        _chatPanelView.SetMessages(messages);
    }
}
