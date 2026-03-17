using Godot;

public partial class PlayerView : Control
{
    [Signal]
    public delegate void PlayerSelectedEventHandler();

    private readonly Color _inkColor = Color.FromHtml("#2B2726");
    private readonly Color _textColor = Color.FromHtml("#16222B");
    private readonly Color _avatarFillColor = Color.FromHtml("#EAE6E0");
    private readonly Color _passBubbleColor = Color.FromHtml("#D8D3CB");
    private readonly Color _passTextColor = Color.FromHtml("#61F41E");

    private int _slot;
    private string _progress = "0.0%";
    private bool _isPassing;

    private Label _slotLabel = null!;
    private Label _progressLabel = null!;
    private Panel _passBubble = null!;

    public void Configure(int slot, string progress, bool isPassing)
    {
        _slot = slot;
        _progress = progress;
        _isPassing = isPassing;

        if (IsNodeReady())
        {
            ApplyState();
        }
    }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Pass;
        Size = new Vector2(180, 122);
        CustomMinimumSize = Size;

        var avatar = CreateRoundedPanel(new Vector2(0, 0), new Vector2(88, 88), _avatarFillColor, 44, _inkColor, 2);
        AddChild(avatar);

        _slotLabel = CreateTextLabel("P1", 22, _textColor, new Vector2(0, 26), new Vector2(88, 30), HorizontalAlignment.Center);
        AddChild(_slotLabel);

        var clickArea = new Button();
        clickArea.Position = Vector2.Zero;
        clickArea.Size = new Vector2(88, 88);
        clickArea.Flat = true;
        clickArea.Text = string.Empty;
        clickArea.Modulate = new Color(1, 1, 1, 0);
        clickArea.FocusMode = FocusModeEnum.None;
        clickArea.MouseDefaultCursorShape = CursorShape.PointingHand;
        clickArea.Pressed += () => EmitSignal(SignalName.PlayerSelected);
        AddChild(clickArea);

        _passBubble = CreateRoundedPanel(new Vector2(96, 2), new Vector2(76, 40), _passBubbleColor, 18);
        _passBubble.AddChild(CreateTextLabel("PASS", 16, _passTextColor, new Vector2(0, 8), new Vector2(76, 22), HorizontalAlignment.Center));
        AddChild(_passBubble);

        _progressLabel = CreateTextLabel("0.0%", 16, _textColor, new Vector2(0, 92), new Vector2(88, 22), HorizontalAlignment.Center);
        AddChild(_progressLabel);

        ApplyState();
    }

    private void ApplyState()
    {
        _slotLabel.Text = $"P{_slot}";
        _progressLabel.Text = _progress;
        _passBubble.Visible = _isPassing;
    }

    private static Panel CreateRoundedPanel(
        Vector2 position,
        Vector2 size,
        Color fillColor,
        int radius,
        Color? borderColor = null,
        int borderWidth = 0
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

        if (borderWidth > 0 && borderColor.HasValue)
        {
            style.BorderColor = borderColor.Value;
            style.BorderWidthLeft = borderWidth;
            style.BorderWidthTop = borderWidth;
            style.BorderWidthRight = borderWidth;
            style.BorderWidthBottom = borderWidth;
        }

        panel.AddThemeStyleboxOverride("panel", style);
        return panel;
    }

    private static Label CreateTextLabel(
        string text,
        int fontSize,
        Color fontColor,
        Vector2 position,
        Vector2 size,
        HorizontalAlignment alignment
    )
    {
        var label = new Label();
        label.Text = text;
        label.Position = position;
        label.Size = size;
        label.HorizontalAlignment = alignment;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", fontColor);
        return label;
    }
}
