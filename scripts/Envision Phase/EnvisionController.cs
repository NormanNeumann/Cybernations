using Godot;
using System.Collections.Generic;

public partial class EnvisionController : Node
{
	private int currentPlayer = 0;
	private int localPlayerId = 0;

	private List<PlayerState> players = new();

	private ActionPopup popup = null!;
	private StatusBanner banner = null!;

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

		StartTurn();
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
		banner.ShowMessage("Your turn. Choose an action.", new Color("86EFAC"));
	}
	else
	{
		popup.Hide();
		banner.ShowMessage($"Player {currentPlayer + 1} is choosing an action...", new Color("FDE68A"));
	}
}

	private void OnActionChosen(EnvisionAction action)
{
	var player = players[currentPlayer];

	ApplyAction(player, action);

	popup.Hide();
	banner.ShowTemporaryMessage($"Player {currentPlayer + 1} chose: {action}", 2.0f, new Color("7DD3FC"));

	// NextPlayer();
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
