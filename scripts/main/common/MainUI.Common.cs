using Godot;

public partial class MainUI
{
    private Panel CreateInfoPanel(
        Vector2 position,
        Vector2 size,
        string title,
        string body,
        Color fillColor,
        int radius
    )
    {
        var panel = CreateRoundedPanel(position, size, fillColor, radius);
        panel.ClipContents = true;

        var layout = new VBoxContainer();
        layout.Position = new Vector2(18, 14);
        layout.Size = new Vector2(size.X - 36, size.Y - 28);
        layout.AddThemeConstantOverride("separation", 10);
        layout.ClipContents = true;
        panel.AddChild(layout);

        var titleLabel = new Label();
        titleLabel.Text = title;
        titleLabel.CustomMinimumSize = new Vector2(0, 30);
        titleLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        titleLabel.HorizontalAlignment = HorizontalAlignment.Left;
        titleLabel.VerticalAlignment = VerticalAlignment.Center;
        titleLabel.AddThemeFontSizeOverride("font_size", 22);
        titleLabel.AddThemeColorOverride("font_color", _textColor);
        layout.AddChild(titleLabel);

        var bodyLabel = new Label();
        bodyLabel.Text = body;
        bodyLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        bodyLabel.HorizontalAlignment = HorizontalAlignment.Left;
        bodyLabel.VerticalAlignment = VerticalAlignment.Top;
        bodyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        bodyLabel.ClipText = true;
        bodyLabel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        bodyLabel.AddThemeFontSizeOverride("font_size", 14);
        bodyLabel.AddThemeColorOverride("font_color", _textColor);
        layout.AddChild(bodyLabel);

        return panel;
    }

    private Panel CreateRoundedPanel(
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

    private Panel CreateRoundPanel(
        Vector2 position,
        Vector2 size,
        Color fillColor,
        int radius,
        Color borderColor,
        int borderWidth
    )
    {
        return CreateRoundedPanel(position, size, fillColor, radius, borderColor, borderWidth);
    }

    private Panel CreateStatusBubble(Vector2 position, string text)
    {
        var bubble = CreateRoundedPanel(position, new Vector2(76, 40), _passBubbleColor, 18);
        bubble.AddChild(CreateTextLabel(text, 16, _passTextColor, new Vector2(0, 8), new Vector2(76, 22), HorizontalAlignment.Center));
        return bubble;
    }

    private Label CreateTextLabel(
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
