using Godot;
using System;

public partial class FeedbackTokenPopup : Control
{
	public Action<string>? OnTokenConfirmed;
	public Action? OnCancelled;

	private Label _title = null!;
	private Label _subtitle = null!;

	private Button _wildsButton = null!;
	private Button _wastesButton = null!;
	private Button _worksButton = null!;
	private Button _agoraButton = null!;
	private Button _developButton = null!;
	private Button _transformButton = null!;

	private Button _confirmButton = null!;
	private Button _cancelButton = null!;

	private string selectedToken = "";

	public override void _Ready()
	{
		_title = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/TitleLabel");
		_subtitle = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/SubtitleLabel");

		_wildsButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/TokenButtonsVBox/WildsButton");
		_wastesButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/TokenButtonsVBox/WastesButton");
		_worksButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/TokenButtonsVBox/WorksButton");
		_agoraButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/TokenButtonsVBox/AgoraButton");
		_developButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/TokenButtonsVBox/DevelopButton");
		_transformButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/TokenButtonsVBox/TransformButton");

		_confirmButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/ConfirmButton");
		_cancelButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/CancelButton");

		_wildsButton.Pressed += () => SelectToken("Wilds");
		_wastesButton.Pressed += () => SelectToken("Wastes");
		_worksButton.Pressed += () => SelectToken("Works");
		_agoraButton.Pressed += () => SelectToken("Agora");
		_developButton.Pressed += () => SelectToken("Develop");
		_transformButton.Pressed += () => SelectToken("Transform");

		_confirmButton.Pressed += Confirm;
		_cancelButton.Pressed += Cancel;

		Hide();
	}

	public void Open()
	{
		Show();

		selectedToken = "";

		_title.Text = "Choose Feedback Token";
		_subtitle.Text = "Select 1 feedback token to add from Reserve to Bag.";

		_wildsButton.Text = "Wilds Feedback";
		_wastesButton.Text = "Wastes Feedback";
		_worksButton.Text = "Works Feedback";
		_agoraButton.Text = "Agora Feedback";
		_developButton.Text = "Develop Feedback";
		_transformButton.Text = "Transform Feedback";

		_confirmButton.Disabled = true;
	}

	private void SelectToken(string tokenType)
	{
		selectedToken = tokenType;
		_subtitle.Text = $"Selected: {tokenType} Feedback";
		_confirmButton.Disabled = false;
	}

	private void Confirm()
	{
		if (selectedToken == "")
			return;

		Hide();
		OnTokenConfirmed?.Invoke(selectedToken);
	}

	private void Cancel()
	{
		Hide();
		OnCancelled?.Invoke();
	}
}
