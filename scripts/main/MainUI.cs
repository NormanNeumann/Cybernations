using System;
using Godot;
//using Cybernations.Application.ViewModels;
using Cybernations.Scripts.Main.Components;


public partial class MainUI : Control
{
	private static readonly Vector2 LayoutShift = new Vector2(40.0f, 0.0f);
	private const float HexOuterSide = 112.0f;

	private PlayerDetailPopupView _playerDetailPopupView = null!;
	private MainUiPresenter _presenter = null!;
	private IGameGateway _gameGateway = null!;
	private event Action<int, string, Vector2>? PlayerCardSelected;
	[Export]
	public string ServerUrl { get; set; } = "";

	//private EnvisionPhasePanelView _envisionPhasePanel = null!;
	//private EnvisionPhaseVm _currentEnvisionVm = new();
	private Control _chatPanel;
	private EnvisionController _envisionController = null!;

	private readonly PlayerData[] _players =
	{
		new PlayerData(1, "0.0%", true),
		new PlayerData(2, "66.7%", true),
		new PlayerData(3, "10.3%", false),
		new PlayerData(4, "90.7%", true),
		new PlayerData(5, "33.3%", false),
	};

	private readonly HexTileData[] _hexTiles =
	{
		new HexTileData(new Vector2(0, 0), HexBase.Wilds, OverlayType.None),
		new HexTileData(new Vector2(348, 0), HexBase.Wilds, OverlayType.None),
		new HexTileData(new Vector2(696, 0), HexBase.Wilds, OverlayType.None),
		new HexTileData(new Vector2(174, 100), HexBase.Wasted, OverlayType.None),
		new HexTileData(new Vector2(522, 100), HexBase.Wilds, OverlayType.None),
		new HexTileData(new Vector2(348, 200), HexBase.Wilds, OverlayType.Human),
		new HexTileData(new Vector2(522, 300), HexBase.Wasted, OverlayType.Human),
		new HexTileData(new Vector2(174, 300), HexBase.Wilds, OverlayType.None),
		new HexTileData(new Vector2(0, 400), HexBase.Wilds, OverlayType.None),
		new HexTileData(new Vector2(348, 400), HexBase.Wasted, OverlayType.Tech),
		new HexTileData(new Vector2(696, 400), HexBase.Wasted, OverlayType.Tech),
	};

	public override void _Ready()
	{
		var playerPanel = GetNode<VBoxContainer>("UIMain/PlayerPanel");
		var gameBoard = GetNode<Node2D>("World/GameBoard");
		var teamGoalPanelView = GetNode<TeamGoalPanelView>("UIMain/TeamGoalPanel");
		var chatPanelView = GetNode<ChatPanelView>("UIMain/ChatPanel");
		_chatPanel = chatPanelView;
		_playerDetailPopupView = GetNode<PlayerDetailPopupView>("UIMain/Popups/PlayerDetailPopup");

		_gameGateway = new WebSocketGameGateway(ServerUrl);
		_presenter = new MainUiPresenter(chatPanelView, teamGoalPanelView, _playerDetailPopupView, _gameGateway);
		_presenter.Initialize();
		PlayerCardSelected += _presenter.OnPlayerSelected;
		
		_envisionController = GetNode<EnvisionController>("EnvisionController");
		_envisionController.PopupOpened += DimBackground;
		_envisionController.PopupClosed += RestoreBackground;

		 //_envisionPhasePanel = GetNode<EnvisionPhasePanelView>("UIMain/EnvisionPhasePanel");
			//BindEnvisionPhaseSignals();
			//LoadFakeEnvisionPhase();

		playerPanel.Position = Shift(new Vector2(34, 38));
		playerPanel.Size = new Vector2(180, 760);
		playerPanel.AddThemeConstantOverride("separation", 28);

		BuildPlayerColumn(playerPanel);
		BuildHexCluster(gameBoard);
	}
	
	private void DimBackground()
{
	if (_chatPanel != null)
		_chatPanel.Modulate = new Color(1, 1, 1, 0.5f); 
}

private void RestoreBackground()
{
	if (_chatPanel != null)
		_chatPanel.Modulate = new Color(1, 1, 1, 1f); 
}

	public override void _ExitTree()
	{
		if (_envisionController != null)
		{
		_envisionController.PopupOpened -= DimBackground;
		_envisionController.PopupClosed -= RestoreBackground;
		}
		if (_presenter == null)
		{
			return;
		}

		PlayerCardSelected -= _presenter.OnPlayerSelected;
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

	//private void BindEnvisionPhaseSignals()
	//{
		//_envisionPhasePanel.ShiftPowerPressed += OnShiftPowerPressed;
		//_envisionPhasePanel.ComeTogetherPressed += OnComeTogetherPressed;
		//_envisionPhasePanel.ConnectPressed += OnConnectPressed;
		//_envisionPhasePanel.SetCoursePressed += OnSetCoursePressed;
		//_envisionPhasePanel.PreparePressed += OnPreparePressed;
		//_envisionPhasePanel.SteerPressed += OnSteerPressed;
		//_envisionPhasePanel.PassPressed += OnPassPressed;
	//}

	//private void OnShiftPowerPressed()
	//{
		//GD.Print("Envision Action: Shift Power");
		//UpdateStatus("Shift Power selected.");
	//}
//
	//private void OnComeTogetherPressed()
	//{
		//GD.Print("Envision Action: Come Together");
		//UpdateStatus("Come Together selected.");
	//}
//
	//private void OnConnectPressed()
	//{
		//GD.Print("Envision Action: Connect");
		//UpdateStatus("Connect selected.");
	//}
//
	//private void OnSetCoursePressed()
	//{
		//GD.Print("Envision Action: Set Course");
		//UpdateStatus("Set Course selected.");
	//}
//
	//private void OnPreparePressed()
	//{
		//GD.Print("Envision Action: Prepare");
		//UpdateStatus("Prepare selected.");
	//}
//
	//private void OnSteerPressed()
	//{
		//GD.Print("Envision Action: Steer");
		//UpdateStatus("Steer selected.");
	//}
//
	//private void OnPassPressed()
	//{
		//GD.Print("Envision Action: Pass");
		//UpdateStatus("Player passed.");
	//}

	//private void UpdateStatus(string message)
	//{
		//_currentEnvisionVm = new EnvisionPhaseVm
		//{
			//IsVisible = _currentEnvisionVm.IsVisible,
			//PhaseTitle = _currentEnvisionVm.PhaseTitle,
			//ActivePlayerText = _currentEnvisionVm.ActivePlayerText,
			//PhaseHintText = _currentEnvisionVm.PhaseHintText,
			//ActionCostText = _currentEnvisionVm.ActionCostText,
			//FeedbackSlots = _currentEnvisionVm.FeedbackSlots,
			//CanShiftPower = _currentEnvisionVm.CanShiftPower,
			//CanComeTogether = _currentEnvisionVm.CanComeTogether,
			//CanConnect = _currentEnvisionVm.CanConnect,
			//CanSetCourse = _currentEnvisionVm.CanSetCourse,
			//CanPrepare = _currentEnvisionVm.CanPrepare,
			//CanSteer = _currentEnvisionVm.CanSteer,
			//CanPass = _currentEnvisionVm.CanPass,
			//StatusMessage = message
		//};

		//_envisionPhasePanel.Configure(_currentEnvisionVm);
	//}

	//private void LoadFakeEnvisionPhase()
	//{
		//_currentEnvisionVm = new EnvisionPhaseVm
		//{
			//IsVisible = true,
			//PhaseTitle = "Envision Phase",
			//ActivePlayerText = "Current Player: P1",
			//PhaseHintText = "Players take actions clockwise until all players pass.",
			//ActionCostText = "Current extra cost: none",
			//StatusMessage = "Choose an action.",
			//FeedbackSlots = new[]
			//{
				//"Wilds",
				//"Wastes",
				//"Develop",
				//"Agora",
				//"-",
				//"-",
				//"-",
				//"-",
				//"-",
				//"-",
				//"-"
			//},
			//CanShiftPower = true,
			//CanComeTogether = true,
			//CanConnect = true,
			//CanSetCourse = true,
			//CanPrepare = true,
			//CanSteer = true,
			//CanPass = true
		//};

		//_envisionPhasePanel.Configure(_currentEnvisionVm);
	//}

	//private void RefreshEnvisionAvailability(
		//bool canShiftPower,
		//bool canComeTogether,
		//bool canConnect,
		//bool canSetCourse,
		//bool canPrepare,
		//bool canSteer,
		//bool canPass)
	//{
		//_currentEnvisionVm = new EnvisionPhaseVm
		//{
			//IsVisible = _currentEnvisionVm.IsVisible,
			//PhaseTitle = _currentEnvisionVm.PhaseTitle,
			//ActivePlayerText = _currentEnvisionVm.ActivePlayerText,
			//PhaseHintText = _currentEnvisionVm.PhaseHintText,
			//ActionCostText = _currentEnvisionVm.ActionCostText,
			//StatusMessage = _currentEnvisionVm.StatusMessage,
			//FeedbackSlots = _currentEnvisionVm.FeedbackSlots,
			//CanShiftPower = canShiftPower,
			//CanComeTogether = canComeTogether,
			//CanConnect = canConnect,
			//CanSetCourse = canSetCourse,
			//CanPrepare = canPrepare,
			//CanSteer = canSteer,
			//CanPass = canPass
		//};
//
		//_envisionPhasePanel.Configure(_currentEnvisionVm);
	//}

	//RefreshEnvisionAvailability(
		//canShiftPower: true,
		//canComeTogether: false,
		//canConnect: true,
		//canSetCourse: true,
		//canPrepare: false,
		//canSteer: true,
		//canPass: true
	//);

	private static Vector2 Shift(Vector2 position)
	{
		return position + LayoutShift;
	}

	private readonly struct PlayerData
	{
		public PlayerData(int slot, string individualProcess, bool isPassing)
		{
			Slot = slot;
			IndividualProcess = individualProcess;
			IsPassing = isPassing;
		}

		public int Slot { get; }
		public string IndividualProcess { get; }
		public bool IsPassing { get; }
	}

	private readonly struct HexTileData
	{
		public HexTileData(Vector2 position, HexBase @base, OverlayType overlay)
		{
			Position = position;
			Base = @base;
			Overlay = overlay;
		}

		public Vector2 Position { get; }
		public HexBase Base { get; }
		public OverlayType Overlay { get; }
	}

	private enum HexBase
	{
		Wilds,
		Wasted,
	}

	private enum OverlayType
	{
		None,
		Human,
		Tech,
	}
}
