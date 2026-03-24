using Godot;
using System;
using System.Collections.Generic;

public partial class TargetPlayerPopup : Control
{
	public Action<int>? OnTargetPlayerSelected;
	public Action? OnCancelled;

	private readonly List<Button> _playerButtons = new();
	private Button _cancelButton = null!;

	public override void _Ready()
	{
		_playerButtons.Add(GetNode<Button>("PopupPanel/MarginContainer/RootVBox/PlayerButtonsVBox/Player1Button"));
		_playerButtons.Add(GetNode<Button>("PopupPanel/MarginContainer/RootVBox/PlayerButtonsVBox/Player2Button"));
		_playerButtons.Add(GetNode<Button>("PopupPanel/MarginContainer/RootVBox/PlayerButtonsVBox/Player3Button"));
		_playerButtons.Add(GetNode<Button>("PopupPanel/MarginContainer/RootVBox/PlayerButtonsVBox/Player4Button"));
		_playerButtons.Add(GetNode<Button>("PopupPanel/MarginContainer/RootVBox/PlayerButtonsVBox/Player5Button"));

		_cancelButton = GetNode<Button>("PopupPanel/MarginContainer/RootVBox/CancelButton");

		for (int i = 0; i < _playerButtons.Count; i++)
		{
			int playerIndex = i;
			_playerButtons[i].Pressed += () => SelectPlayer(playerIndex);
		}

		_cancelButton.Pressed += CancelSelection;

		Hide();
	}

	public void Configure(int totalPlayers, int currentPlayerId)
	{
		Show();

		for (int i = 0; i < _playerButtons.Count; i++)
		{
			bool active = i < totalPlayers;
			_playerButtons[i].Visible = active;

			if (!active)
				continue;

			_playerButtons[i].Text = $"Player {i + 1}";
			
			_playerButtons[i].Disabled = false;
		}
	}

	private void SelectPlayer(int playerId)
	{
		Hide();
		OnTargetPlayerSelected?.Invoke(playerId);
	}

	private void CancelSelection()
	{
		Hide();
		OnCancelled?.Invoke();
	}
}
