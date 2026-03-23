using Godot;
using System;
using System.Collections.Generic;

public partial class ActionPopup : Control
{
	public Action<EnvisionAction> OnActionSelected;

	private Dictionary<EnvisionAction, Button> buttons;

	public override void _Ready()
	{
		buttons = new Dictionary<EnvisionAction, Button>
		{
			{ EnvisionAction.ShiftPower, GetNode<Button>("Panel/VBoxContainer/ShiftPower") },
			{ EnvisionAction.ComeTogether, GetNode<Button>("Panel/VBoxContainer/ComeTogether") },
			{ EnvisionAction.Connect, GetNode<Button>("Panel/VBoxContainer/Connect") },
			{ EnvisionAction.SetCourse, GetNode<Button>("Panel/VBoxContainer/SetCourse") },
			{ EnvisionAction.Prepare, GetNode<Button>("Panel/VBoxContainer/Prepare") },
			{ EnvisionAction.Steer, GetNode<Button>("Panel/VBoxContainer/Steer") },
			{ EnvisionAction.Pass, GetNode<Button>("Panel/VBoxContainer/Pass") },
		};

		foreach (var kv in buttons)
		{
			var action = kv.Key;
			kv.Value.Pressed += () => SelectAction(action);
		}
		
		Position = new Vector2(400, 200);
	}

	public void UpdateButtons(PlayerState player)
	{
		foreach (var kv in buttons)
		{
			bool canUse = ActionCostChecker.CanExecute(player, kv.Key);

			kv.Value.Disabled = !canUse;
			kv.Value.Modulate = canUse ? Colors.White : new Color(0.4f, 0.4f, 0.4f);
		}
	}

	private void SelectAction(EnvisionAction action)
	{
		OnActionSelected?.Invoke(action);
		Hide();
	}
}
