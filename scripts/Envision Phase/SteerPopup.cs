using Godot;
using System;

public partial class SteerPopup : Control
{
	public Action<string>? OnSteerConfirmed;
	public Action? OnCancelled;

	private Label _title = null!;
	private Label _subtitle = null!;
	private Button _addReserveTokenButton = null!;
	private Button _manipulateTokensButton = null!;
	private Button _confirmButton = null!;
	private Button _cancelButton = null!;

	private string selectedMode = "";

	public override void _Ready()
	{
		_title = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/TitleLabel");
		_subtitle = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/SubtitleLabel");

		_addReserveTokenButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/OptionsVBox/AddReserveTokenButton");
		_manipulateTokensButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/OptionsVBox/ManipulateTokensButton");

		_confirmButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/ConfirmButton");
		_cancelButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/CancelButton");

		_addReserveTokenButton.Pressed += () => SelectMode("AddReserveToken");
		_manipulateTokensButton.Pressed += () => SelectMode("ManipulateTokens");

		_confirmButton.Pressed += Confirm;
		_cancelButton.Pressed += Cancel;

		Hide();
	}

	public void Open()
	{
		Show();

		selectedMode = "";

		_title.Text = "Steer";
		_subtitle.Text = "Choose how to use Steer.";

		_addReserveTokenButton.Text = "Add 1 Feedback Token from Reserve to Bag";
		_manipulateTokensButton.Text = "Manipulate Track and Bag Tokens";

		_confirmButton.Disabled = true;
	}

	private void SelectMode(string mode)
	{
		selectedMode = mode;

		_subtitle.Text = mode == "AddReserveToken"
			? "Selected: Add 1 Feedback Token from Reserve to Bag"
			: "Selected: Manipulate Track and Bag Tokens";

		_confirmButton.Disabled = false;
	}

	private void Confirm()
	{
		if (selectedMode == "")
			return;

		Hide();
		OnSteerConfirmed?.Invoke(selectedMode);
	}

	private void Cancel()
	{
		Hide();
		OnCancelled?.Invoke();
	}
}
