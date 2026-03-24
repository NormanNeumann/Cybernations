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
	
	public event Action? PopupOpened;
	public event Action? PopupClosed;

	public override void _Ready()
	{
		GD.Print("EnvisionController Ready");

		popup = GetNode<ActionPopup>("../UIMain/ActionPopup");
		banner = GetNode<StatusBanner>("../UIMain/StatusBanner");

		popup.OnActionSelected += OnActionChosen;

		players = new List<PlayerState>
		{
			new PlayerState
			{
				Id = 0,
				People = 2,
				Environment = 2,
				Technology = 1,
				Cybernation = 0,
				Cohesion = 5
			},
			new PlayerState
			{
				Id = 1,
				People = 0,
				Environment = 0,
				Technology = 2,
				Cybernation = 0,
				Cohesion = 5
			}
		};
		
		popup.Hide();
		banner.Hide();
		
		targetPlayerPopup = GetNode<TargetPlayerPopup>("../UIMain/Popups/TargetPlayerPopup");

		targetPlayerPopup.OnTargetPlayerSelected += OnShiftPowerTargetSelected;
		targetPlayerPopup.OnCancelled += OnTargetSelectionCancelled;

		//StartTurn();
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
			banner.ShowMessage("Choose a target player for Shift Power.", new Color("FDE68A"));
			return;

		case EnvisionAction.ComeTogether:
		case EnvisionAction.Connect:
		case EnvisionAction.SetCourse:
		case EnvisionAction.Prepare:
		case EnvisionAction.Steer:
		case EnvisionAction.Pass:
			ApplyAction(player, action);
			popup.Hide();
			PopupClosed?.Invoke();
			banner.ShowTemporaryMessage($"Player {currentPlayer + 1} chose: {action}", 2.0f, new Color("7DD3FC"));
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
