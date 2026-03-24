using Godot;

public partial class InfoSummaryPanelView : Control, IPopupHostAwareView
{
    private readonly Color _textColor = Color.FromHtml("#16222B");

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Size = new Vector2(500, 292);
        CustomMinimumSize = Size;

        var panel = CreateRoundedPanel(Vector2.Zero, Size, Color.FromHtml("#CFCFCF"), 30);
        panel.ClipContents = true;
        AddChild(panel);

        var layout = new VBoxContainer();
        layout.Position = new Vector2(18, 14);
        layout.Size = new Vector2(Size.X - 36, Size.Y - 28);
        layout.AddThemeConstantOverride("separation", 10);
        layout.ClipContents = true;
        panel.AddChild(layout);

        var title = new Label();
        title.Text = "Information Panel";
        title.CustomMinimumSize = new Vector2(0, 30);
        title.AddThemeFontSizeOverride("font_size", 22);
        title.AddThemeColorOverride("font_color", _textColor);
        layout.AddChild(title);

        var body = new Label();
        body.Text = "Key state summary:\n- 5 players on the left\n- 11 map hexes in the center\n- 3 resource tracks at the bottom";
        body.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        body.ClipText = true;
        body.SizeFlagsVertical = SizeFlags.ExpandFill;
        body.AddThemeFontSizeOverride("font_size", 14);
        body.AddThemeColorOverride("font_color", _textColor);
        layout.AddChild(body);
    }

    private static Panel CreateRoundedPanel(
        Vector2 position,
        Vector2 size,
        Color fillColor,
        int radius
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
        panel.AddThemeStyleboxOverride("panel", style);
        return panel;
    }

    public void SetPopupHost(Control popupHost)
    {
        // Reserved for the future expanded popup variant of this panel.
    }
}
