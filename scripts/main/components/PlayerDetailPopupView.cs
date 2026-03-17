using Godot;

public partial class PlayerDetailPopupView : Control
{
    private readonly Color _inkColor = Color.FromHtml("#2B2726");
    private readonly Color _textColor = Color.FromHtml("#16222B");
    private readonly Color _mutedTextColor = Color.FromHtml("#4B5E69");

    private Panel _panel = null!;
    private Label _titleLabel = null!;
    private Label _bodyLabel = null!;

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Stop;
        AnchorRight = 1.0f;
        AnchorBottom = 1.0f;
        GrowHorizontal = GrowDirection.Both;
        GrowVertical = GrowDirection.Both;
        Visible = false;

        _panel = CreateRoundedPanel(Vector2.Zero, new Vector2(650, 400), Color.FromHtml("#F4F4F4"), 18, _inkColor, 2);
        _panel.Visible = false;
        _panel.ZIndex = 100;
        _panel.ClipContents = true;
        _panel.MouseFilter = MouseFilterEnum.Pass;
        AddChild(_panel);

        var layout = new VBoxContainer();
        layout.Position = new Vector2(24, 20);
        layout.Size = new Vector2(602, 356);
        layout.AddThemeConstantOverride("separation", 12);
        _panel.AddChild(layout);

        _titleLabel = new Label();
        _titleLabel.Text = "Player detail";
        _titleLabel.CustomMinimumSize = new Vector2(0, 36);
        _titleLabel.AddThemeFontSizeOverride("font_size", 30);
        _titleLabel.AddThemeColorOverride("font_color", _textColor);
        layout.AddChild(_titleLabel);

        _bodyLabel = new Label();
        _bodyLabel.Text = string.Empty;
        _bodyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _bodyLabel.SizeFlagsVertical = SizeFlags.ExpandFill;
        _bodyLabel.AddThemeFontSizeOverride("font_size", 20);
        _bodyLabel.AddThemeColorOverride("font_color", _mutedTextColor);
        layout.AddChild(_bodyLabel);

        var closeButton = new Button();
        closeButton.Text = "x";
        closeButton.Position = new Vector2(600, 14);
        closeButton.Size = new Vector2(36, 32);
        closeButton.Flat = true;
        closeButton.FocusMode = FocusModeEnum.None;
        closeButton.MouseDefaultCursorShape = CursorShape.PointingHand;
        closeButton.AddThemeFontSizeOverride("font_size", 28);
        closeButton.AddThemeColorOverride("font_color", _inkColor);
        closeButton.Pressed += HidePopup;
        _panel.AddChild(closeButton);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Visible || !_panel.Visible)
        {
            return;
        }

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouseButton)
        {
            return;
        }

        if (GetGlobalRect(_panel).HasPoint(mouseButton.GlobalPosition))
        {
            return;
        }

        HidePopup();
        GetViewport().SetInputAsHandled();
    }

    public void ShowPopup(int slot, string progress, Vector2 preferredPosition)
    {
        _titleLabel.Text = "Player detail";
        _bodyLabel.Text =
            $"Player {slot}\n" +
            $"Progress: {progress}\n\n" +
            "More player data can be rendered here.";

        var viewportSize = GetViewportRect().Size;
        var clampedPosition = new Vector2(
            Mathf.Clamp(preferredPosition.X, 0, viewportSize.X - _panel.Size.X),
            Mathf.Clamp(preferredPosition.Y, 0, viewportSize.Y - _panel.Size.Y)
        );

        Visible = true;
        _panel.Position = clampedPosition;
        _panel.Visible = true;
    }

    public void HidePopup()
    {
        _panel.Visible = false;
        Visible = false;
    }

    private static Panel CreateRoundedPanel(
        Vector2 position,
        Vector2 size,
        Color fillColor,
        int radius,
        Color borderColor,
        int borderWidth
    )
    {
        var panel = new Panel();
        panel.Position = position;
        panel.Size = size;

        var style = new StyleBoxFlat();
        style.BgColor = fillColor;
        style.CornerRadiusTopLeft = radius;
        style.CornerRadiusTopRight = radius;
        style.CornerRadiusBottomLeft = radius;
        style.CornerRadiusBottomRight = radius;
        style.BorderColor = borderColor;
        style.BorderWidthLeft = borderWidth;
        style.BorderWidthTop = borderWidth;
        style.BorderWidthRight = borderWidth;
        style.BorderWidthBottom = borderWidth;
        panel.AddThemeStyleboxOverride("panel", style);
        return panel;
    }

    private static Rect2 GetGlobalRect(Control control)
    {
        return new Rect2(control.GlobalPosition, control.Size);
    }
}
