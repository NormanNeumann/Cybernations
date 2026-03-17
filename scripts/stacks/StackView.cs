using Godot;

public partial class StackView : Node2D
{
    public enum StackBaseKind
    {
        Wilds,
        Wasted,
    }

    public enum StackOverlayKind
    {
        None,
        Human,
        Tech,
    }

    private readonly Color _inkColor = Color.FromHtml("#2B2726");
    private readonly Color _wildsColor = Color.FromHtml("#6CE575");
    private readonly Color _wastedColor = Color.FromHtml("#D07D29");
    private readonly Color _humanColor = Color.FromHtml("#C92CC1");
    private readonly Color _techColor = Color.FromHtml("#3D29ED");
    private readonly Color _highlightOuterColor = Color.FromHtml("#EEF55D");
    private readonly Color _highlightInnerColor = Color.FromHtml("#E2C54D");
    private readonly Color _highlightConflictColor = Color.FromHtml("#F82D23");

    public StackBaseKind BaseKind { get; private set; } = StackBaseKind.Wilds;
    public StackOverlayKind OverlayKind { get; private set; } = StackOverlayKind.None;
    public bool ConflictHighlight { get; private set; }

    public float OuterSide { get; private set; } = 112.0f;
    public float InnerSide { get; private set; } = 108.0f;
    public float OverlayOuterSide { get; private set; } = 84.0f;
    public float OverlayInnerSide { get; private set; } = 80.0f;

    public void Configure(
        StackBaseKind baseKind,
        StackOverlayKind overlayKind,
        bool conflictHighlight,
        float outerSide = 112.0f,
        float innerSide = 108.0f,
        float overlayOuterSide = 84.0f,
        float overlayInnerSide = 80.0f
    )
    {
        BaseKind = baseKind;
        OverlayKind = overlayKind;
        ConflictHighlight = conflictHighlight;
        OuterSide = outerSide;
        InnerSide = innerSide;
        OverlayOuterSide = overlayOuterSide;
        OverlayInnerSide = overlayInnerSide;
        QueueRedraw();
    }

    public override void _Draw()
    {
        var center = GetHexBounds(OuterSide) / 2.0f;

        if (ConflictHighlight)
        {
            DrawPolygon(BuildRegularHexPolygon(OuterSide + 7.0f, center), new[] { _highlightOuterColor });
            DrawPolygon(BuildRegularHexPolygon(OuterSide + 3.0f, center), new[] { _highlightInnerColor });
            DrawPolygon(BuildRegularHexPolygon(OuterSide, center), new[] { _inkColor });
            DrawPolygon(BuildRegularHexPolygon(InnerSide, center), new[] { _highlightConflictColor });
            return;
        }

        var baseColor = BaseKind == StackBaseKind.Wilds ? _wildsColor : _wastedColor;
        DrawPolygon(BuildRegularHexPolygon(OuterSide, center), new[] { _inkColor });
        DrawPolygon(BuildRegularHexPolygon(InnerSide, center), new[] { baseColor });

        if (OverlayKind == StackOverlayKind.None)
        {
            return;
        }

        var overlayColor = OverlayKind == StackOverlayKind.Human ? _humanColor : _techColor;
        DrawPolygon(BuildRegularHexPolygon(OverlayOuterSide, center), new[] { _inkColor });
        DrawPolygon(BuildRegularHexPolygon(OverlayInnerSide, center), new[] { overlayColor });
    }

    private static Vector2 GetHexBounds(float sideLength)
    {
        return new Vector2(sideLength * 2.0f, Mathf.Sqrt(3.0f) * sideLength);
    }

    private static Vector2[] BuildRegularHexPolygon(float sideLength, Vector2 center)
    {
        var halfHeight = Mathf.Sqrt(3.0f) * sideLength * 0.5f;
        var halfSide = sideLength * 0.5f;

        return new[]
        {
            new Vector2(center.X + sideLength, center.Y),
            new Vector2(center.X + halfSide, center.Y + halfHeight),
            new Vector2(center.X - halfSide, center.Y + halfHeight),
            new Vector2(center.X - sideLength, center.Y),
            new Vector2(center.X - halfSide, center.Y - halfHeight),
            new Vector2(center.X + halfSide, center.Y - halfHeight),
        };
    }
}
