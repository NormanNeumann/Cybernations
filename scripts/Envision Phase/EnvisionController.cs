using Godot;
using System.Collections.Generic;
using System;

public partial class EnvisionController : Node
{
	private int currentPlayer = 0;
	private int localPlayerId = 0;

	private List<PlayerState> players = new();

	private ActionPopup popup = null!;
	private StatusBanner banner = null!;
	
	private TargetPlayerPopup targetPlayerPopup = null!;
	private EnvisionAction? pendingAction = null;
	
	private ConnectPopup connectPopup = null!;
	private SetCoursePopup setCoursePopup = null!;
	private SteerPopup steerPopup = null!;
	
	public event Action? PopupOpened;
	public event Action? PopupClosed;

	public override void _Ready()
{
	GD.Print("EnvisionController Ready");

	popup = GetNode<ActionPopup>("../UIMain/ActionPopup");
	banner = GetNode<StatusBanner>("../UIMain/StatusBanner");
	targetPlayerPopup = GetNode<TargetPlayerPopup>("../UIMain/Popups/TargetPlayerPopup");
	connectPopup = GetNode<ConnectPopup>("../UIMain/Popups/ConnectPopup");

	popup.OnActionSelected += OnActionChosen;

	targetPlayerPopup.OnTargetPlayerSelected += OnShiftPowerTargetSelected;
	targetPlayerPopup.OnCancelled += OnTargetSelectionCancelled;

	connectPopup.OnConnectConfirmed += OnConnectConfirmed;
	connectPopup.OnCancelled += OnConnectCancelled;

	players = new List<PlayerState>
{
	new PlayerState { Id = 0, People = 2, Environment = 2, Technology = 2, Cybernation = 0, Cohesion = 5 },
	new PlayerState { Id = 1, People = 0, Environment = 0, Technology = 2, Cybernation = 0, Cohesion = 5 },
	new PlayerState { Id = 2, People = 1, Environment = 1, Technology = 1, Cybernation = 0, Cohesion = 5 },
	new PlayerState { Id = 3, People = 3, Environment = 1, Technology = 0, Cybernation = 0, Cohesion = 5 },
	new PlayerState { Id = 4, People = 1, Environment = 2, Technology = 2, Cybernation = 0, Cohesion = 5 }
};

	setCoursePopup = GetNode<SetCoursePopup>("../UIMain/Popups/SetCoursePopup");

	setCoursePopup.OnSetCourseConfirmed += OnSetCourseConfirmed;
	setCoursePopup.OnCancelled += OnSetCourseCancelled;
	
	steerPopup = GetNode<SteerPopup>("../UIMain/Popups/SteerPopup");

	steerPopup.OnSteerConfirmed += OnSteerConfirmed;
	steerPopup.OnCancelled += OnSteerCancelled;

	popup.Hide();
	banner.Hide();
	targetPlayerPopup.Hide();
	connectPopup.Hide();
	setCoursePopup.Hide();
	steerPopup.Hide();
}

	public override void _UnhandledInput(InputEvent @event)
{
	if (@event.IsActionPressed("test_player_1"))
	{
		localPlayerId = 0;
		GD.Print("Switched local view to Player 1");
		StartTurn();
	}

	if (@event.IsActionPressed("test_player_2"))
	{
		localPlayerId = 1;
		GD.Print("Switched local view to Player 2");
		StartTurn();
	}
	
	if (@event.IsActionPressed("test_turn_player_1"))
	{
		currentPlayer = 0;
		GD.Print("Current turn set to Player 1");
		StartTurn();
	}

	if (@event.IsActionPressed("test_turn_player_2"))
	{
		currentPlayer = 1;
		GD.Print("Current turn set to Player 2");
		StartTurn();
	}
}

	private void StartTurn()
{
	var player = players[currentPlayer];

	banner.ClearMessage();

	if (currentPlayer == localPlayerId)
	{
		popup.Show();
		popup.UpdateButtons(player);
		PopupOpened?.Invoke();
		banner.ShowMessage("Your turn. Choose an action.", new Color("86EFAC"));
	}
	else
	{
		popup.Hide();
		PopupClosed?.Invoke();
		banner.ShowMessage($"Player {currentPlayer + 1} is choosing an action...", new Color("FDE68A"));
	}
}

	private void OnActionChosen(EnvisionAction action)
{
	var player = players[currentPlayer];

	switch (action)
	{
		case EnvisionAction.ShiftPower:
			pendingAction = action;

			popup.Hide();
			targetPlayerPopup.Configure(players.Count, currentPlayer);
			PopupOpened?.Invoke();

			banner.ShowMessage("Choose a target player for Shift Power.", new Color("FDE68A"));
			return;

		case EnvisionAction.ComeTogether:
			ApplyAction(player, action);
			popup.Hide();
			PopupClosed?.Invoke();

			banner.ShowTemporaryMessage(
				$"Player {currentPlayer + 1} chose: {action}",
				2.0f,
				new Color("7DD3FC")
			);
			return;

		case EnvisionAction.Connect:
			pendingAction = action;

			popup.Hide();
			connectPopup.Open(player);
			PopupOpened?.Invoke();

			banner.ShowMessage("Choose relationships for Connect.", new Color("FDE68A"));
			return;

		case EnvisionAction.SetCourse:
			pendingAction = action;

			popup.Hide();
			setCoursePopup.Open();
			PopupOpened?.Invoke();

			banner.ShowMessage("Choose how to use Set Course.", new Color("FDE68A"));
			return;
			
		case EnvisionAction.Prepare:
		
		case EnvisionAction.Steer:
			pendingAction = action;

			popup.Hide();
			steerPopup.Open();

			banner.ShowMessage("Choose how to use Steer.", new Color("FDE68A"));
			return;
			
		case EnvisionAction.Pass:
			ApplyAction(player, action);
			popup.Hide();
			PopupClosed?.Invoke();

			banner.ShowTemporaryMessage(
				$"Player {currentPlayer + 1} chose: {action}",
				2.0f,
				new Color("7DD3FC")
			);
			return;
	}
}

	private void OnShiftPowerTargetSelected(int targetPlayerId)
	{
		if (pendingAction != EnvisionAction.ShiftPower)
			return;

		var player = players[currentPlayer];

		ApplyAction(player, EnvisionAction.ShiftPower);

		pendingAction = null;

		PopupClosed?.Invoke();

		banner.ShowTemporaryMessage(
			$"Player {currentPlayer + 1} chose: Shift Power -> Player {targetPlayerId + 1}",
			2.0f,
			new Color("7DD3FC")
		);

		GD.Print($"Shift Power target selected: Player {targetPlayerId + 1}");
	}
	
		private void OnTargetSelectionCancelled()
	{
		pendingAction = null;

		popup.Show();
		popup.UpdateButtons(players[currentPlayer]);

		banner.ShowMessage("Shift Power cancelled. Choose an action.", new Color("86EFAC"));
	}
	
	private void OnConnectConfirmed(string spendType, string gainType)
{
	var player = players[currentPlayer];

	if (spendType == "People") player.People -= 2;
	if (spendType == "Environment") player.Environment -= 2;
	if (spendType == "Technology") player.Technology -= 2;

	if (gainType == "People") player.People += 1;
	if (gainType == "Environment") player.Environment += 1;
	if (gainType == "Technology") player.Technology += 1;

	pendingAction = null;

	PopupClosed?.Invoke();

	banner.ShowTemporaryMessage(
		$"Player {currentPlayer + 1} chose: Connect ({spendType} → {gainType})",
		2.0f,
		new Color("7DD3FC")
	);
}

private void OnConnectCancelled()
{
	pendingAction = null;

	popup.Show();
	popup.UpdateButtons(players[currentPlayer]);

	banner.ShowMessage("Connect cancelled. Choose an action.", new Color("86EFAC"));
}

private void OnSetCourseConfirmed(string mode)
{
	var player = players[currentPlayer];

	// 扣除 Set Course 的费用
	ApplyAction(player, EnvisionAction.SetCourse);

	pendingAction = null;

	PopupClosed?.Invoke();

	if (mode == "MovePeople")
	{
		banner.ShowTemporaryMessage(
			$"Player {currentPlayer + 1} chose: Set Course -> Move People Token",
			2.0f,
			new Color("7DD3FC")
		);

		GD.Print("Set Course selected: Move People Token");
	}
	else if (mode == "RotateStack")
	{
		banner.ShowTemporaryMessage(
			$"Player {currentPlayer + 1} chose: Set Course -> Rotate Stack",
			2.0f,
			new Color("7DD3FC")
		);

		GD.Print("Set Course selected: Rotate Stack");
	}
}

private void OnSetCourseCancelled()
{
	pendingAction = null;

	popup.Show();
	popup.UpdateButtons(players[currentPlayer]);

	banner.ShowMessage("Set Course cancelled. Choose an action.", new Color("86EFAC"));
}

private void OnSteerConfirmed(string mode)
{
	var player = players[currentPlayer];

	// 扣除 Steer 的费用
	ApplyAction(player, EnvisionAction.Steer);

	pendingAction = null;

	PopupClosed?.Invoke();

	if (mode == "AddReserveToken")
	{
		banner.ShowTemporaryMessage(
			$"Player {currentPlayer + 1} chose: Steer -> Add Reserve Token",
			2.0f,
			new Color("7DD3FC")
		);

		GD.Print("Steer selected: Add Reserve Token");
	}
	else if (mode == "ManipulateTokens")
	{
		banner.ShowTemporaryMessage(
			$"Player {currentPlayer + 1} chose: Steer -> Manipulate Tokens",
			2.0f,
			new Color("7DD3FC")
		);

		GD.Print("Steer selected: Manipulate Tokens");
	}
}

private void OnSteerCancelled()
{
	pendingAction = null;

	popup.Show();
	popup.UpdateButtons(players[currentPlayer]);

	banner.ShowMessage("Steer cancelled. Choose an action.", new Color("86EFAC"));
}

	private void NextPlayer()
	{
		currentPlayer = (currentPlayer + 1) % players.Count;
		StartTurn();
	}

	private void ApplyAction(PlayerState player, EnvisionAction action)
	{
		switch (action)
		{
			case EnvisionAction.ShiftPower:
				player.People -= 1;
				break;

			case EnvisionAction.ComeTogether:
				player.Environment -= 1;
				player.Cohesion += 1;
				break;

			case EnvisionAction.Connect:
				// 暂时占位，后面你可以补更细的关系扣除逻辑
				break;

			case EnvisionAction.SetCourse:
				player.Technology -= 2;
				break;

			case EnvisionAction.Prepare:
				player.People -= 2;
				player.Cybernation += 1;
				break;

			case EnvisionAction.Steer:
				player.Environment -= 2;
				break;

			case EnvisionAction.Pass:
				break;
		}
	}
}
