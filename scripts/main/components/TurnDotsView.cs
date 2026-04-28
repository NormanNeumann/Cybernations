using Godot;

public partial class TurnDotsView : Control
{
	private const int TotalTurns = 6;

	private readonly Color _inkColor = Color.FromHtml("#2B2726");
	private readonly Color _inactiveColor = Color.FromHtml("#ECEAE7");
	private readonly Color _activeColor = Color.FromHtml("#3D29ED");

	[Export]
	public int CompletedTurns { get; set; }

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Ignore;
		Size = new Vector2(150, 98);
		CustomMinimumSize = Size;

		var dotSize = new Vector2(38, 38);
		const float gapX = 48.0f;
		const float gapY = 52.0f;

		for (int index = 0; index < TotalTurns; index++)
		{
			var row = index / 3;
			var col = index % 3;
			var fill = index < CompletedTurns ? _activeColor : _inactiveColor;
			AddChild(CreateRoundPanel(new Vector2(gapX * col, gapY * row), dotSize, fill, 19, _inkColor, 2));
		}
	}

	private static Panel CreateRoundPanel(
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
}
