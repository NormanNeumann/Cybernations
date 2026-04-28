using Godot;

public partial class NationLevelBadgeView : Control
{
	private readonly Color _inkColor = Color.FromHtml("#2B2726");
	private readonly Color _fillColor = Color.FromHtml("#F0B54B");

	[Export]
	public string LevelText { get; set; } = "10";

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Ignore;
		Size = new Vector2(112, 140);
		CustomMinimumSize = Size;

		var outline = new Polygon2D();
		outline.Color = _inkColor;
		outline.Polygon = BuildShieldPolygon(new Vector2(112, 140), Vector2.Zero);
		AddChild(outline);

		var fill = new Polygon2D();
		fill.Color = _fillColor;
		fill.Polygon = BuildShieldPolygon(new Vector2(102, 130), new Vector2(5, 5));
		AddChild(fill);

		var value = new Label();
		value.Text = LevelText;
		value.Position = new Vector2(6, 18);
		value.Size = new Vector2(100, 64);
		value.HorizontalAlignment = HorizontalAlignment.Center;
		value.VerticalAlignment = VerticalAlignment.Center;
		value.AddThemeFontSizeOverride("font_size", 58);
		value.AddThemeColorOverride("font_color", Colors.Black);
		AddChild(value);
	}

	private static Vector2[] BuildShieldPolygon(Vector2 size, Vector2 offset)
	{
		return
		[
			new Vector2(offset.X + size.X * 0.12f, offset.Y),
			new Vector2(offset.X + size.X * 0.88f, offset.Y),
			new Vector2(offset.X + size.X, offset.Y + size.Y * 0.22f),
			new Vector2(offset.X + size.X, offset.Y + size.Y * 0.64f),
			new Vector2(offset.X + size.X * 0.5f, offset.Y + size.Y),
			new Vector2(offset.X, offset.Y + size.Y * 0.64f),
			new Vector2(offset.X, offset.Y + size.Y * 0.22f),
		];
	}
}
