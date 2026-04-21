using Godot;
//using Cybernations.Application.ViewModels;
using Cybernations.Scripts.Main.Components;


public partial class MainUI : Control
{
    private PlayerDetailPopupView _playerDetailPopupView = null!;
    private PlayerPanelView _playerPanelView = null!;
    private MainUiPresenter _presenter = null!;
    private IGameGateway _gameGateway = null!;
    private EnvisionController? _envisionController;
    private ColorRect _background = null!;
    private ColorRect _colorblindFilter = null!;
    private Button _colorblindToggleButton = null!;
    private Color _backgroundBaseColor = Colors.White;
    private bool _isDimmed;

    [Export]
    public string ServerUrl { get; set; } = "";

    public override void _Ready()
    {
        _background = GetNode<ColorRect>("Background");
        _backgroundBaseColor = _background.Color;
        _colorblindFilter = GetNode<ColorRect>("ColorblindOverlay/Filter");
        _colorblindToggleButton = GetNode<Button>("UIMain/ColorblindToggleButton");
        _envisionController = GetNodeOrNull<EnvisionController>("EnvisionController");

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

        _colorblindToggleButton.Pressed += AccessibilityManager.CycleMode;
        AccessibilityManager.OnAccessibilityChanged += UpdateAccessibilityUi;
        UpdateAccessibilityUi();

        if (_envisionController != null)
        {
            _envisionController.PopupOpened += DimBackground;
            _envisionController.PopupClosed += RestoreBackground;
        }
    }

	public override void _ExitTree()
	{
		if (_envisionController != null)
		{
            _envisionController.PopupOpened -= DimBackground;
            _envisionController.PopupClosed -= RestoreBackground;
		}
        AccessibilityManager.OnAccessibilityChanged -= UpdateAccessibilityUi;
        if (_colorblindToggleButton != null)
        {
            _colorblindToggleButton.Pressed -= AccessibilityManager.CycleMode;
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

    private void UpdateAccessibilityUi()
    {
        var modeText = AccessibilityManager.CurrentMode switch
        {
            AccessibilityMode.Off => "Off",
            AccessibilityMode.GlobalFilter => "Global Filter",
            AccessibilityMode.BoardRecolor => "Board Recolor",
            _ => "Off"
        };
        _colorblindToggleButton.Text = $"Colorblind Mode: {modeText}";

        if (AccessibilityManager.IsGlobalFilterEnabled)
        {
            _colorblindFilter.Visible = true;
            _colorblindFilter.Color = new Color(1.0f, 0.9f, 0.75f, 0.22f);
        }
        else
        {
            _colorblindFilter.Visible = false;
        }
    }

    private void DimBackground()
    {
        _isDimmed = true;
        _background.Color = _backgroundBaseColor.Darkened(0.35f);
    }

    private void RestoreBackground()
    {
        _isDimmed = false;
        _background.Color = _backgroundBaseColor;
    }
}
