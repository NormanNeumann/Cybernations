using Godot;
//using Cybernations.Application.ViewModels;
using Cybernations.Scripts.Main.Components;


public partial class MainUI : Control
{
    private PlayerDetailPopupView _playerDetailPopupView = null!;
    private PlayerPanelView _playerPanelView = null!;
    private MainUiPresenter _presenter = null!;
    private IGameGateway _gameGateway = null!;

    [Export]
    public string ServerUrl { get; set; } = "";

    public override void _Ready()
    {
        _playerPanelView = GetNode<PlayerPanelView>("UIMain/PlayerPanel");
        var teamGoalPanelView = GetNode<TeamGoalPanelView>("UIMain/TeamGoalPanel");
        var infoSummaryPanelView = GetNode<InfoSummaryPanelView>("UIMain/InfoSummaryPanel");
        var chatPanelView = GetNode<ChatPanelView>("UIMain/ChatPanel");
        var hiveBoardView = GetNode<HiveBoardView>("World/GameBoard");
        var teamGoalPopupHost = GetNode<Control>("UIMain/Popups/TeamGoalPopupHost");
        var infoSummaryPopupHost = GetNode<Control>("UIMain/Popups/InfoSummaryPopupHost");
        var chatPopupHost = GetNode<Control>("UIMain/Popups/ChatPopupHost");
        _playerDetailPopupView = GetNode<PlayerDetailPopupView>("UIMain/Popups/PlayerDetailPopup");

        teamGoalPanelView.SetPopupHost(teamGoalPopupHost);
        infoSummaryPanelView.SetPopupHost(infoSummaryPopupHost);
        chatPanelView.SetPopupHost(chatPopupHost);

        _gameGateway = ServerUrl.Trim().Length == 0
            ? new LoopbackGameGateway()
            : new WebSocketGameGateway(ServerUrl);
        _presenter = new MainUiPresenter(
            chatPanelView,
            teamGoalPanelView,
            infoSummaryPanelView,
            hiveBoardView,
            _playerDetailPopupView,
            _gameGateway
        );
        _presenter.Initialize();
        _playerPanelView.PlayerSelected += _presenter.OnPlayerSelected;
    }

	public override void _ExitTree()
	{
		if (_envisionController != null)
		{
		AccessibilityManager.OnAccessibilityChanged -= UpdateAccessibilityUi;
		_envisionController.PopupOpened -= DimBackground;
		_envisionController.PopupClosed -= RestoreBackground;
		}
		if (_presenter == null)
		{
			return;
		}

        if (_playerPanelView != null)
        {
            _playerPanelView.PlayerSelected -= _presenter.OnPlayerSelected;
        }
        _presenter.Dispose();
        _gameGateway.Shutdown();
    }

	public override void _Process(double delta)
	{
		if (_gameGateway == null)
		{
			return;
		}

        _gameGateway.Poll();
    }
}
