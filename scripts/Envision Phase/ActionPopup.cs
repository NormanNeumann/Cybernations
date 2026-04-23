using Godot;
using System;
using System.Collections.Generic;

public partial class ActionPopup : Control
{
	public Action<EnvisionAction> OnActionSelected = delegate { };

	private Dictionary<EnvisionAction, Button> buttons = null!;
	private Label _actionDescription = null!;

	public override void _Ready()
	{
		buttons = new Dictionary<EnvisionAction, Button>
		{
			{ EnvisionAction.ShiftPower, GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ButtonsGrid/ShiftPower") },
			{ EnvisionAction.ComeTogether, GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ButtonsGrid/ComeTogether") },
			{ EnvisionAction.Connect, GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ButtonsGrid/Connect") },
			{ EnvisionAction.SetCourse, GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ButtonsGrid/SetCourse") },
			{ EnvisionAction.Prepare, GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ButtonsGrid/Prepare") },
			{ EnvisionAction.Steer, GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ButtonsGrid/Steer") },
			{ EnvisionAction.Pass, GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ButtonsGrid/Pass") },
		};

		_actionDescription = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/ActionDescription");

		foreach (var kv in buttons)
		{
			var action = kv.Key;
			kv.Value.Pressed += () => SelectAction(action);
		}

		BindHoverDescriptions();
	}

	private void BindHoverDescriptions()
	{
		buttons[EnvisionAction.ShiftPower].MouseEntered += () =>
			_actionDescription.Text = "Shift Power: Spend 1 People Relationship to give the First Player token to any player.";

		buttons[EnvisionAction.ComeTogether].MouseEntered += () =>
			_actionDescription.Text = "Come Together: Spend 1 Environment Relationship to gain 1 Cohesion.";

		buttons[EnvisionAction.Connect].MouseEntered += () =>
			_actionDescription.Text = "Connect: Spend 2 of the same Relationship to gain 1 Relationship of your choice.";

		buttons[EnvisionAction.SetCourse].MouseEntered += () =>
			_actionDescription.Text = "Set Course: Spend 2 Technology Relationship to move 1 People token or rotate 1 Stack.";

		buttons[EnvisionAction.Prepare].MouseEntered += () =>
			_actionDescription.Text = "Prepare: Spend 2 People Relationship to gain 1 Cybernation level.";

		buttons[EnvisionAction.Steer].MouseEntered += () =>
			_actionDescription.Text = "Steer: Spend 2 Environment Relationship to manipulate Feedback tokens.";

		buttons[EnvisionAction.Pass].MouseEntered += () =>
			_actionDescription.Text = "Pass: Take no further action this Envision Phase.";
	}

	public void UpdateButtons(PlayerState player)
	{
		foreach (var kv in buttons)
		{
			bool canUse = ActionCostChecker.CanExecute(player, kv.Key);
			kv.Value.Disabled = !canUse;
		}
	}

	private void SelectAction(EnvisionAction action)
	{
		OnActionSelected.Invoke(action);
		Hide();
	}
	
	public void SetActionAvailability(
	bool canShiftPower,
	bool canComeTogether,
	bool canConnect,
	bool canSetCourse,
	bool canPrepare,
	bool canSteer,
	bool canPass
)
{
	buttons[EnvisionAction.ShiftPower].Disabled = !canShiftPower;
	buttons[EnvisionAction.ComeTogether].Disabled = !canComeTogether;
	buttons[EnvisionAction.Connect].Disabled = !canConnect;
	buttons[EnvisionAction.SetCourse].Disabled = !canSetCourse;
	buttons[EnvisionAction.Prepare].Disabled = !canPrepare;
	buttons[EnvisionAction.Steer].Disabled = !canSteer;
	buttons[EnvisionAction.Pass].Disabled = !canPass;
}
}
