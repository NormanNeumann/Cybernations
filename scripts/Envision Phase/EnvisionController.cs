using Godot;
using System;
using System.Collections.Generic;

public partial class EnvisionController : Node
{
	private int currentPlayer = 0;
	private int localPlayerId = 0;

	// 仍然保留本地测试数据，作为过渡期 debug mode
	private List<PlayerState> players = new();

	private ActionPopup popup = null!;
	private StatusBanner banner = null!;

	private TargetPlayerPopup targetPlayerPopup = null!;
	private ConnectPopup connectPopup = null!;
	private SetCoursePopup setCoursePopup = null!;
	private SteerPopup steerPopup = null!;
	private FeedbackTokenPopup feedbackTokenPopup = null!;

	private EnvisionAction? pendingAction = null;
	private EnvisionUiState? _state;

	public event Action? PopupOpened;
	public event Action? PopupClosed;

	public event Action<EnvisionActionRequest>? ActionRequested;

	public override void _Ready()
	{
		GD.Print("EnvisionController Ready");

		popup = GetNode<ActionPopup>("../UIMain/Popups/ActionPopup");
		banner = GetNode<StatusBanner>("../UIMain/Popups/StatusBanner");

		targetPlayerPopup = GetNode<TargetPlayerPopup>("../UIMain/Popups/TargetPlayerPopup");
		connectPopup = GetNode<ConnectPopup>("../UIMain/Popups/ConnectPopup");
		setCoursePopup = GetNode<SetCoursePopup>("../UIMain/Popups/SetCoursePopup");
		steerPopup = GetNode<SteerPopup>("../UIMain/Popups/SteerPopup");
		feedbackTokenPopup = GetNode<FeedbackTokenPopup>("../UIMain/Popups/FeedbackTokenPopup");

		popup.OnActionSelected += OnActionChosen;

		targetPlayerPopup.OnTargetPlayerSelected += OnShiftPowerTargetSelected;
		targetPlayerPopup.OnCancelled += OnTargetSelectionCancelled;

		connectPopup.OnConnectConfirmed += OnConnectConfirmed;
		connectPopup.OnCancelled += OnConnectCancelled;

		setCoursePopup.OnSetCourseConfirmed += OnSetCourseConfirmed;
		setCoursePopup.OnCancelled += OnSetCourseCancelled;

		steerPopup.OnSteerConfirmed += OnSteerConfirmed;
		steerPopup.OnCancelled += OnSteerCancelled;

		feedbackTokenPopup.OnTokenConfirmed += OnFeedbackTokenConfirmed;
		feedbackTokenPopup.OnCancelled += OnFeedbackTokenCancelled;

		players = new List<PlayerState>
		{
			new PlayerState { Id = 0, People = 2, Environment = 2, Technology = 2, Cybernation = 0, Cohesion = 5 },
			new PlayerState { Id = 1, People = 0, Environment = 0, Technology = 2, Cybernation = 0, Cohesion = 5 },
			new PlayerState { Id = 2, People = 1, Environment = 1, Technology = 1, Cybernation = 0, Cohesion = 5 },
			new PlayerState { Id = 3, People = 3, Environment = 1, Technology = 0, Cybernation = 0, Cohesion = 5 },
			new PlayerState { Id = 4, People = 1, Environment = 2, Technology = 2, Cybernation = 0, Cohesion = 5 }
		};

		HideAllPopups();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("test_player_1"))
		{
			localPlayerId = 0;
			GD.Print("Switched local view to Player 1");
			StartDebugTurn();
		}

		if (@event.IsActionPressed("test_player_2"))
		{
			localPlayerId = 1;
			GD.Print("Switched local view to Player 2");
			StartDebugTurn();
		}

		if (@event.IsActionPressed("test_turn_player_1"))
		{
			currentPlayer = 0;
			GD.Print("Current turn set to Player 1");
			StartDebugTurn();
		}

		if (@event.IsActionPressed("test_turn_player_2"))
		{
			currentPlayer = 1;
			GD.Print("Current turn set to Player 2");
			StartDebugTurn();
		}
	}

	public void ApplyState(EnvisionUiState state)
	{
		_state = state;

		if (!state.IsVisible)
		{
			HideAllPopups();
			PopupClosed?.Invoke();

			if (!string.IsNullOrWhiteSpace(state.StatusMessage))
			{
				banner.ShowTemporaryMessage(state.StatusMessage, 2.0f, new Color("7DD3FC"));
			}
			else
			{
				banner.Hide();
			}

			return;
		}

		HideSecondaryPopups();

		if (state.IsLocalPlayersTurn)
		{
			popup.Show();

			ApplyPopupAvailabilityFromState();

			PopupOpened?.Invoke();
			banner.ShowMessage(
				string.IsNullOrWhiteSpace(state.StatusMessage)
					? "Your turn. Choose an action."
					: state.StatusMessage,
				new Color("86EFAC")
			);
		}
		else
		{
			popup.Hide();
			PopupClosed?.Invoke();
			banner.ShowMessage(
				string.IsNullOrWhiteSpace(state.StatusMessage)
					? $"Player {state.CurrentPlayerId + 1} is choosing an action..."
					: state.StatusMessage,
				new Color("FDE68A")
			);
		}
	}

	private void StartDebugTurn()
	{
		var state = new EnvisionUiState
		{
			IsVisible = true,
			CurrentPlayerId = currentPlayer,
			LocalPlayerId = localPlayerId,
			IsLocalPlayersTurn = currentPlayer == localPlayerId,
			Players = players.ToArray(),
			CanShiftPower = true,
			CanComeTogether = false,
			CanConnect = true,
			CanSetCourse = true,
			CanPrepare = false,
			CanSteer = true,
			CanPass = true,
			StatusMessage = currentPlayer == localPlayerId
				? "Your turn. Choose an action."
				: $"Player {currentPlayer + 1} is choosing an action..."
		};

		ApplyState(state);
	}

	private void OnActionChosen(EnvisionAction action)
	{
		switch (action)
		{
			case EnvisionAction.ShiftPower:
				pendingAction = action;
				popup.Hide();
				targetPlayerPopup.Configure(GetCurrentPlayers().Length, GetCurrentPlayerId());
				PopupOpened?.Invoke();
				banner.ShowMessage("Choose a target player for Shift Power.", new Color("FDE68A"));
				return;

			case EnvisionAction.ComeTogether:
				PopupClosed?.Invoke();
				ActionRequested?.Invoke(new EnvisionActionRequest
				{
					Action = "ComeTogether"
				});
				return;

			case EnvisionAction.Connect:
				pendingAction = action;
				popup.Hide();
				connectPopup.Open(GetCurrentPlayerState());
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
				PopupClosed?.Invoke();
				ActionRequested?.Invoke(new EnvisionActionRequest
				{
					Action = "Prepare"
				});
				return;

			case EnvisionAction.Steer:
				pendingAction = action;
				popup.Hide();
				steerPopup.Open();
				PopupOpened?.Invoke();
				banner.ShowMessage("Choose how to use Steer.", new Color("FDE68A"));
				return;

			case EnvisionAction.Pass:
				PopupClosed?.Invoke();
				ActionRequested?.Invoke(new EnvisionActionRequest
				{
					Action = "Pass"
				});
				return;
		}
	}

	private void OnShiftPowerTargetSelected(int targetPlayerId)
	{
		if (pendingAction != EnvisionAction.ShiftPower)
		{
			return;
		}

		pendingAction = null;
		PopupClosed?.Invoke();

		ActionRequested?.Invoke(new EnvisionActionRequest
		{
			Action = "ShiftPower",
			TargetPlayerId = targetPlayerId
		});
	}

	private void OnTargetSelectionCancelled()
	{
		pendingAction = null;
		popup.Show();
		ApplyPopupAvailabilityFromState();
		PopupOpened?.Invoke();
		banner.ShowMessage("Shift Power cancelled. Choose an action.", new Color("86EFAC"));
	}

	private void OnConnectConfirmed(string spendType, string gainType)
	{
		pendingAction = null;
		PopupClosed?.Invoke();

		ActionRequested?.Invoke(new EnvisionActionRequest
		{
			Action = "Connect",
			SpendType = spendType,
			GainType = gainType
		});
	}

	private void OnConnectCancelled()
	{
		pendingAction = null;
		popup.Show();
		ApplyPopupAvailabilityFromState();
		PopupOpened?.Invoke();
		banner.ShowMessage("Connect cancelled. Choose an action.", new Color("86EFAC"));
	}

	private void OnSetCourseConfirmed(string mode)
	{
		pendingAction = null;
		PopupClosed?.Invoke();

		ActionRequested?.Invoke(new EnvisionActionRequest
		{
			Action = "SetCourse",
			Mode = mode
		});
	}

	private void OnSetCourseCancelled()
	{
		pendingAction = null;
		popup.Show();
		ApplyPopupAvailabilityFromState();
		PopupOpened?.Invoke();
		banner.ShowMessage("Set Course cancelled. Choose an action.", new Color("86EFAC"));
	}

	private void OnSteerConfirmed(string mode)
	{
		pendingAction = EnvisionAction.Steer;

		if (mode == "AddReserveToken")
		{
			steerPopup.Hide();
			feedbackTokenPopup.Open();
			PopupOpened?.Invoke();
			banner.ShowMessage("Choose a feedback token to add from Reserve to Bag.", new Color("FDE68A"));
			return;
		}

		if (mode == "ManipulateTokens")
		{
			pendingAction = null;
			PopupClosed?.Invoke();

			ActionRequested?.Invoke(new EnvisionActionRequest
			{
				Action = "Steer",
				Mode = "ManipulateTokens"
			});
		}
	}

	private void OnSteerCancelled()
	{
		pendingAction = null;
		popup.Show();
		ApplyPopupAvailabilityFromState();
		PopupOpened?.Invoke();
		banner.ShowMessage("Steer cancelled. Choose an action.", new Color("86EFAC"));
	}

	private void OnFeedbackTokenConfirmed(string tokenType)
	{
		pendingAction = null;
		PopupClosed?.Invoke();

		ActionRequested?.Invoke(new EnvisionActionRequest
		{
			Action = "Steer",
			Mode = "AddReserveToken",
			FeedbackTokenType = tokenType
		});
	}

	private void OnFeedbackTokenCancelled()
	{
		feedbackTokenPopup.Hide();
		steerPopup.Open();
		PopupOpened?.Invoke();
		banner.ShowMessage("Feedback token selection cancelled. Choose how to use Steer.", new Color("86EFAC"));
	}

	private void HideSecondaryPopups()
	{
		targetPlayerPopup.Hide();
		connectPopup.Hide();
		setCoursePopup.Hide();
		steerPopup.Hide();
		feedbackTokenPopup.Hide();
	}

	private void HideAllPopups()
	{
		popup.Hide();
		targetPlayerPopup.Hide();
		connectPopup.Hide();
		setCoursePopup.Hide();
		steerPopup.Hide();
		feedbackTokenPopup.Hide();
		banner.Hide();
	}

	private PlayerState GetCurrentPlayerState()
	{
		var allPlayers = GetCurrentPlayers();
		var index = GetCurrentPlayerId();

		if (allPlayers.Length == 0 || index < 0 || index >= allPlayers.Length)
		{
			return new PlayerState();
		}

		return allPlayers[index];
	}

	private PlayerState[] GetCurrentPlayers()
	{
		if (_state != null && _state.Players.Length > 0)
		{
			return _state.Players;
		}

		return players.ToArray();
	}

	private int GetCurrentPlayerId()
	{
		if (_state != null)
		{
			return _state.CurrentPlayerId;
		}

		return currentPlayer;
	}
	
	private void ApplyPopupAvailabilityFromState()
{
	if (_state == null)
	{
		return;
	}

	popup.SetActionAvailability(
		_state.CanShiftPower,
		_state.CanComeTogether,
		_state.CanConnect,
		_state.CanSetCourse,
		_state.CanPrepare,
		_state.CanSteer,
		_state.CanPass
	);
}
}
