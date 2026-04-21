using Godot;
using System;

public partial class SetCoursePopup : Control
{
	public Action<string>? OnSetCourseConfirmed;
	public Action? OnCancelled;

	private Label _title = null!;
	private Label _subtitle = null!;
	private Button _movePeopleButton = null!;
	private Button _rotateStackButton = null!;
	private Button _confirmButton = null!;
	private Button _cancelButton = null!;

	private string selectedMode = "";

	public override void _Ready()
	{
		_title = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/TitleLabel");
		_subtitle = GetNode<Label>("PopupPanel/MarginContainer/RootVBox/SubtitleLabel");

		_movePeopleButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/OptionsVBox/MovePeopleButton");
		_rotateStackButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/OptionsVBox/RotateStackButton");

		_confirmButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/ConfirmButton");
		_cancelButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/ActionsRow/CancelButton");

		_movePeopleButton.Pressed += () => SelectMode("MovePeople");
		_rotateStackButton.Pressed += () => SelectMode("RotateStack");

		_confirmButton.Pressed += Confirm;
		_cancelButton.Pressed += Cancel;

		Hide();
	}

	public void Open()
	{
		Show();

		selectedMode = "";

		_title.Text = "Set Course";
		_subtitle.Text = "Choose how to use Set Course.";

		_movePeopleButton.Text = "Move People Token";
		_rotateStackButton.Text = "Rotate Stack";

		_confirmButton.Disabled = true;
	}

	private void SelectMode(string mode)
	{
		selectedMode = mode;
		_subtitle.Text = mode == "MovePeople"
			? "Selected: Move People Token"
			: "Selected: Rotate Stack";

		_confirmButton.Disabled = false;
	}

	private void Confirm()
	{
		if (selectedMode == "")
			return;

		Hide();
		OnSetCourseConfirmed?.Invoke(selectedMode);
	}

	private void Cancel()
	{
		Hide();
		OnCancelled?.Invoke();
	}
}
