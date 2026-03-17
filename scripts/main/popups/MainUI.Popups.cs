using Godot;

public partial class MainUI
{
    private void BuildPlayerDetailPopup(Control parent)
    {
        _playerDetailBackdrop = new Button();
        _playerDetailBackdrop.AnchorRight = 1.0f;
        _playerDetailBackdrop.AnchorBottom = 1.0f;
        _playerDetailBackdrop.GrowHorizontal = Control.GrowDirection.Both;
        _playerDetailBackdrop.GrowVertical = Control.GrowDirection.Both;
        _playerDetailBackdrop.Flat = true;
        _playerDetailBackdrop.Text = string.Empty;
        _playerDetailBackdrop.Modulate = new Color(1, 1, 1, 0);
        _playerDetailBackdrop.FocusMode = Control.FocusModeEnum.None;
        _playerDetailBackdrop.Visible = false;
        _playerDetailBackdrop.ZIndex = 99;
        _playerDetailBackdrop.Pressed += HidePlayerDetail;
        parent.AddChild(_playerDetailBackdrop);

        _playerDetailPanel = CreateRoundedPanel(Vector2.Zero, new Vector2(650, 400), Color.FromHtml("#F4F4F4"), 18, _inkColor, 2);
        _playerDetailPanel.Visible = false;
        _playerDetailPanel.ZIndex = 100;
        _playerDetailPanel.ClipContents = true;
        parent.AddChild(_playerDetailPanel);

        var layout = new VBoxContainer();
        layout.Position = new Vector2(24, 20);
        layout.Size = new Vector2(602, 356);
        layout.AddThemeConstantOverride("separation", 12);
        _playerDetailPanel.AddChild(layout);

        _playerDetailTitleLabel = new Label();
        _playerDetailTitleLabel.Text = "Player detail";
        _playerDetailTitleLabel.CustomMinimumSize = new Vector2(0, 36);
        _playerDetailTitleLabel.AddThemeFontSizeOverride("font_size", 30);
        _playerDetailTitleLabel.AddThemeColorOverride("font_color", _textColor);
        layout.AddChild(_playerDetailTitleLabel);

        _playerDetailBodyLabel = new Label();
        _playerDetailBodyLabel.Text = string.Empty;
        _playerDetailBodyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _playerDetailBodyLabel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        _playerDetailBodyLabel.AddThemeFontSizeOverride("font_size", 20);
        _playerDetailBodyLabel.AddThemeColorOverride("font_color", _mutedTextColor);
        layout.AddChild(_playerDetailBodyLabel);

        var closeButton = new Button();
        closeButton.Text = "x";
        closeButton.Position = new Vector2(600, 14);
        closeButton.Size = new Vector2(36, 32);
        closeButton.Flat = true;
        closeButton.FocusMode = Control.FocusModeEnum.None;
        closeButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        closeButton.AddThemeFontSizeOverride("font_size", 28);
        closeButton.AddThemeColorOverride("font_color", _inkColor);
        closeButton.Pressed += HidePlayerDetail;
        _playerDetailPanel.AddChild(closeButton);
    }

    private void ShowPlayerDetail(PlayerData player, Vector2 preferredPosition)
    {
        _playerDetailTitleLabel.Text = "Player detail";
        _playerDetailBodyLabel.Text =
            $"Player {player.Slot}\n" +
            $"Progress: {player.IndividualProcess}\n\n" +
            "More player data can be rendered here.";

        var viewportSize = GetViewportRect().Size;
        var clampedPosition = new Vector2(
            Mathf.Clamp(preferredPosition.X, 0, viewportSize.X - _playerDetailPanel.Size.X),
            Mathf.Clamp(preferredPosition.Y, 0, viewportSize.Y - _playerDetailPanel.Size.Y)
        );

        _playerDetailBackdrop.Visible = true;
        _playerDetailPanel.Position = clampedPosition;
        _playerDetailPanel.Visible = true;
    }

    private void HidePlayerDetail()
    {
        _playerDetailPanel.Visible = false;
        _playerDetailBackdrop.Visible = false;
    }
}
