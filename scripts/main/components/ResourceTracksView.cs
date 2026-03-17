using Godot;

public partial class ResourceTracksView : Control
{
    private const int TotalResourceCells = 25;
    private const int ConflictCells = 3;

    private readonly Color _inkColor = Color.FromHtml("#2B2726");
    private readonly Color _iconFillColor = Color.FromHtml("#DFD7CE");
    private readonly Color _emptyColor = Color.FromHtml("#D9D9D9");
    private readonly Color _conflictColor = Color.FromHtml("#D46F6F");

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Size = new Vector2(1010, 240);
        CustomMinimumSize = Size;

        AddChild(CreateTrackRow(new Vector2(0, 0), "H", 13, Color.FromHtml("#F4F4F4")));
        AddChild(CreateTrackRow(new Vector2(0, 88), "T", 10, Color.FromHtml("#F4F4F4")));
        AddChild(CreateTrackRow(new Vector2(0, 176), "E", 8, Color.FromHtml("#F4F4F4")));
    }

    private Control CreateTrackRow(Vector2 position, string iconText, int filledCells, Color filledColor)
    {
        var row = new Control();
        row.Position = position;
        row.Size = new Vector2(1010, 64);

        var iconBox = CreateRoundedPanel(Vector2.Zero, new Vector2(72, 72), _iconFillColor, 18);
        row.AddChild(iconBox);
        row.AddChild(CreateTextLabel(iconText, 28, Colors.Black, new Vector2(0, 18), new Vector2(72, 34), HorizontalAlignment.Center));

        const float trackStartX = 102.0f;
        const float cellWidth = 28.0f;
        const float cellHeight = 46.0f;
        const float gap = 8.0f;

        for (int index = 0; index < TotalResourceCells; index++)
        {
            var state = ResolveCellState(index, filledCells);
            var cellColor = state switch
            {
                CellState.Filled => filledColor,
                CellState.Conflict => _conflictColor,
                _ => _emptyColor,
            };

            var radius = index == 0 || index == TotalResourceCells - 1 ? 14 : 4;
            row.AddChild(
                CreateRoundedPanel(
                    new Vector2(trackStartX + index * (cellWidth + gap), 8),
                    new Vector2(cellWidth, cellHeight),
                    cellColor,
                    radius,
                    _inkColor,
                    2
                )
            );
        }

        return row;
    }

    private CellState ResolveCellState(int index, int filledCells)
    {
        if (index >= TotalResourceCells - ConflictCells)
        {
            return CellState.Conflict;
        }

        var maxUsable = TotalResourceCells - ConflictCells;
        return index < Mathf.Min(filledCells, maxUsable) ? CellState.Filled : CellState.Empty;
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

    private enum CellState
    {
        Empty,
        Filled,
        Conflict,
    }
}
