using Godot;
using System;

public partial class ConnectPopup : Control
{
	public Action<string, string>? OnConnectConfirmed;
	public Action? OnCancelled;

	private Label _title = null!;
	private Label _stepLabel = null!;
	private Button _option1 = null!;
	private Button _option2 = null!;
	private Button _option3 = null!;
	private Button _confirm = null!;
	private Button _cancel = null!;

	private string selectedSpend = "";
	private string selectedGain = "";
	private bool selectingSpend = true;

	public override void _Ready()
	{
		_title = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/TitleLabel");
		_stepLabel = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/StepLabel");

		_option1 = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/OptionsVBox/Option1");
		_option2 = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/OptionsVBox/Option2");
		_option3 = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/OptionsVBox/Option3");

		_confirm = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/ConfirmButton");
		_cancel = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/CancelButton");

		_option1.Pressed += () => OnOptionSelected("People");
		_option2.Pressed += () => OnOptionSelected("Environment");
		_option3.Pressed += () => OnOptionSelected("Technology");

		_confirm.Pressed += Confirm;
		_cancel.Pressed += Cancel;

		Hide();
	}

	public void Open(PlayerState player)
{
	Show();

	selectingSpend = true;
	selectedSpend = "";
	selectedGain = "";

	_title.Text = "Connect";
	_stepLabel.Text = "Choose a relationship type to spend (cost: 2)";

	_option1.Text = "People";
	_option2.Text = "Environment";
	_option3.Text = "Technology";

	_option1.Disabled = player.People < 2;
	_option2.Disabled = player.Environment < 2;
	_option3.Disabled = player.Technology < 2;

	_confirm.Disabled = true;
}

	private void OnOptionSelected(string type)
{
	if (selectingSpend)
	{
		selectedSpend = type;
		selectingSpend = false;

		_stepLabel.Text = $"Spend: {selectedSpend} | Now choose a relationship type to gain";

		_option1.Disabled = false;
		_option2.Disabled = false;
		_option3.Disabled = false;

		_confirm.Disabled = true;
	}
	else
	{
		selectedGain = type;
		_stepLabel.Text = $"Spend: {selectedSpend} | Gain: {selectedGain}";
		_confirm.Disabled = false;
	}
}

	private void Confirm()
	{
		if (selectedSpend == "" || selectedGain == "")
			return;

		Hide();
		OnConnectConfirmed?.Invoke(selectedSpend, selectedGain);
	}

	private void Cancel()
	{
		Hide();
		OnCancelled?.Invoke();
	}
}
