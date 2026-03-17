using Godot;

public partial class TeamGoalPanelView : Control
{
    private static readonly Vector2 LayoutShift = new Vector2(40.0f, 0.0f);

    private readonly Color _inkColor = Color.FromHtml("#2B2726");
    private readonly Color _textColor = Color.FromHtml("#16222B");
    private readonly Color _wildsColor = Color.FromHtml("#6CE575");
    private readonly Color _wastedColor = Color.FromHtml("#D07D29");
    private readonly Color _humanOverlayColor = Color.FromHtml("#C92CC1");
    private readonly Color _techOverlayColor = Color.FromHtml("#3D29ED");

    private Panel _previewPanel = null!;
    private Button _hitArea = null!;
    private Button _backdrop = null!;
    private Panel _dropdownPanel = null!;

    private readonly HexTileData[] _hexTiles =
    [
        new HexTileData(new Vector2(0, 0), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(348, 0), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(696, 0), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(174, 100), HexBase.Wasted, OverlayType.None),
        new HexTileData(new Vector2(522, 100), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(348, 200), HexBase.Wilds, OverlayType.Human),
        new HexTileData(new Vector2(522, 300), HexBase.Wasted, OverlayType.Human),
        new HexTileData(new Vector2(174, 300), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(0, 400), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(348, 400), HexBase.Wasted, OverlayType.Tech),
        new HexTileData(new Vector2(696, 400), HexBase.Wasted, OverlayType.Tech),
    ];

    public override void _Ready()
    {
        AnchorRight = 1.0f;
        AnchorBottom = 1.0f;
        GrowHorizontal = GrowDirection.Both;
        GrowVertical = GrowDirection.Both;

        BuildPreviewPanel();
        BuildDropdown();
    }

    private void BuildPreviewPanel()
    {
        _previewPanel = CreateInfoPanel(
            Shift(new Vector2(1305, 34)),
            new Vector2(500, 232),
            "Team Goal",
            "Shared objective for every player:\nStabilize the board, raise nation level, and stop conflict from shrinking the usable tracks.",
            Color.FromHtml("#D7D7D7"),
            0
        );
        AddChild(_previewPanel);

        _hitArea = new Button();
        _hitArea.Position = _previewPanel.Position;
        _hitArea.Size = _previewPanel.Size;
        _hitArea.Flat = true;
        _hitArea.Text = string.Empty;
        _hitArea.Modulate = new Color(1, 1, 1, 0);
        _hitArea.FocusMode = FocusModeEnum.None;
        _hitArea.MouseDefaultCursorShape = CursorShape.PointingHand;
        _hitArea.ZIndex = 82;
        _hitArea.Pressed += ToggleDropdown;
        AddChild(_hitArea);
    }

    private void BuildDropdown()
    {
        _backdrop = new Button();
        _backdrop.AnchorRight = 1.0f;
        _backdrop.AnchorBottom = 1.0f;
        _backdrop.GrowHorizontal = GrowDirection.Both;
        _backdrop.GrowVertical = GrowDirection.Both;
        _backdrop.Flat = true;
        _backdrop.Text = string.Empty;
        _backdrop.Modulate = new Color(1, 1, 1, 0);
        _backdrop.FocusMode = FocusModeEnum.None;
        _backdrop.Visible = false;
        _backdrop.ZIndex = 89;
        _backdrop.Pressed += HideDropdown;
        AddChild(_backdrop);

        _dropdownPanel = CreateRoundedPanel(
            Shift(new Vector2(1045, 34)),
            new Vector2(760, 900),
            Colors.Transparent,
            0
        );
        _dropdownPanel.Visible = false;
        _dropdownPanel.ZIndex = 90;
        _dropdownPanel.ClipContents = false;
        AddChild(_dropdownPanel);

        var sections = new VBoxContainer();
        sections.AnchorRight = 1.0f;
        sections.AnchorBottom = 1.0f;
        sections.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        sections.SizeFlagsVertical = SizeFlags.ExpandFill;
        sections.AddThemeConstantOverride("separation", 14);
        _dropdownPanel.AddChild(sections);

        sections.AddChild(CreateDescriptionSection(new Vector2(760, 300)));
        sections.AddChild(CreateMiniGridSection(new Vector2(760, 360)));
        sections.AddChild(CreateConditionSection(new Vector2(760, 170)));
    }

    private void ToggleDropdown()
    {
        if (_dropdownPanel.Visible)
        {
            HideDropdown();
        }
        else
        {
            ShowDropdown();
        }
    }

    private void ShowDropdown()
    {
        _backdrop.Visible = true;
        _dropdownPanel.Visible = true;
    }

    private void HideDropdown()
    {
        _dropdownPanel.Visible = false;
        _backdrop.Visible = false;
    }

    private Panel CreateDescriptionSection(Vector2 size)
    {
        var section = CreateRoundedPanel(Vector2.Zero, size, Color.FromHtml("#EDEDED"), 44, _inkColor, 3);
        section.CustomMinimumSize = size;
        section.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        section.ClipContents = true;

        var teamGoalTexture = TryLoadTeamGoalTexture();
        if (teamGoalTexture != null)
        {
            var imageRect = new TextureRect();
            imageRect.Position = Vector2.Zero;
            imageRect.Size = size;
            imageRect.Texture = teamGoalTexture;
            imageRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            imageRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
            section.AddChild(imageRect);
            return section;
        }

        var header = new ColorRect();
        header.Position = Vector2.Zero;
        header.Size = new Vector2(size.X, 78);
        header.Color = Color.FromHtml("#2F5CA7");
        section.AddChild(header);

        section.AddChild(CreateTextLabel("RECONNECT", 62, Colors.White, new Vector2(20, 6), new Vector2(size.X - 40, 68), HorizontalAlignment.Center));
        section.AddChild(CreateTextLabel("Victory Condition:", 26, Colors.Black, new Vector2(44, 88), new Vector2(size.X - 88, 32), HorizontalAlignment.Left));
        section.AddChild(CreateTextLabel("- No stacks are wasted", 22, Colors.Black, new Vector2(44, 126), new Vector2(size.X - 88, 28), HorizontalAlignment.Left));
        section.AddChild(CreateTextLabel("- Human / Tech / Environment are each 12 or more", 22, Colors.Black, new Vector2(44, 158), new Vector2(size.X - 88, 28), HorizontalAlignment.Left));
        section.AddChild(CreateTextLabel("Start effects: reconnect fragmented rings", 20, Colors.Black, new Vector2(44, 194), new Vector2(size.X - 88, 28), HorizontalAlignment.Left));
        section.AddChild(
            CreateTextLabel(
                "We sought dynamic balance in nature and artifice.",
                18,
                Color.FromHtml("#666666"),
                new Vector2(30, 238),
                new Vector2(size.X - 60, 44),
                HorizontalAlignment.Center
            )
        );

        return section;
    }

    private Texture2D? TryLoadTeamGoalTexture()
    {
        string[] candidatePaths =
        {
            "res://TeamGoalReconnect.png",
            "res://team_goal_reconnect.png",
            "res://assets/TeamGoalReconnect.png",
            "res://assets/team_goal_reconnect.png",
        };

        foreach (var path in candidatePaths)
        {
            if (!ResourceLoader.Exists(path))
            {
                continue;
            }

            var texture = GD.Load<Texture2D>(path);
            if (texture != null)
            {
                return texture;
            }
        }

        return null;
    }

    private Panel CreateMiniGridSection(Vector2 size)
    {
        var section = CreateRoundedPanel(Vector2.Zero, size, Color.FromHtml("#E9E9E9"), 44, _inkColor, 3);
        section.CustomMinimumSize = size;
        section.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        section.ClipContents = true;

        section.AddChild(CreateTextLabel("Hivegrid Snapshot (Conflict Highlight)", 24, Colors.Black, new Vector2(28, 16), new Vector2(size.X - 56, 34), HorizontalAlignment.Left));

        const float miniOuterSide = 62.0f;
        const float miniInnerSide = 58.0f;
        const float miniOverlayOuterSide = 46.0f;
        const float miniOverlayInnerSide = 42.0f;
        const float miniPositionScale = 0.5f;

        var boardArea = new Rect2(36, 58, size.X - 72, size.Y - 84);
        var miniClusterSize = GetScaledHexClusterSize(miniPositionScale, miniOuterSide);
        var cluster = new Control();
        cluster.Position = new Vector2(
            boardArea.Position.X + (boardArea.Size.X - miniClusterSize.X) * 0.5f,
            boardArea.Position.Y + (boardArea.Size.Y - miniClusterSize.Y) * 0.5f
        );
        cluster.Size = miniClusterSize;
        section.AddChild(cluster);

        for (int index = 0; index < _hexTiles.Length; index++)
        {
            var isConflict = index == 3 || index == 9;
            cluster.AddChild(
                CreateMiniHexTile(
                    _hexTiles[index],
                    _hexTiles[index].Position * miniPositionScale,
                    miniOuterSide,
                    miniInnerSide,
                    miniOverlayOuterSide,
                    miniOverlayInnerSide,
                    isConflict
                )
            );
        }

        return section;
    }

    private Panel CreateConditionSection(Vector2 size)
    {
        var section = CreateRoundedPanel(Vector2.Zero, size, Color.FromHtml("#EFEFEF"), 40, _inkColor, 3);
        section.CustomMinimumSize = size;
        section.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        section.ClipContents = true;

        var rows = new VBoxContainer();
        rows.Position = new Vector2(26, 26);
        rows.Size = new Vector2(size.X - 52, size.Y - 52);
        rows.AddThemeConstantOverride("separation", 14);
        section.AddChild(rows);

        var rowA = new HBoxContainer();
        rowA.AddThemeConstantOverride("separation", 30);
        rowA.AddChild(CreateConditionItem("H", "0 / 12 Not satisfied", Color.FromHtml("#71EFE5")));
        rowA.AddChild(CreateConditionItem("T", "0 / 12 Not satisfied", Color.FromHtml("#71EFE5")));
        rows.AddChild(rowA);

        var rowB = new HBoxContainer();
        rowB.AddThemeConstantOverride("separation", 30);
        rowB.AddChild(CreateConditionItem("E", "0 / 12 Not satisfied", Color.FromHtml("#71EFE5")));
        rowB.AddChild(CreateConditionItem("X", "0 / 0 Satisfied", Color.FromHtml("#F8483F")));
        rows.AddChild(rowB);

        return section;
    }

    private Control CreateConditionItem(string symbol, string statusText, Color symbolColor)
    {
        var item = new HBoxContainer();
        item.AddThemeConstantOverride("separation", 12);

        var icon = CreateRoundedPanel(Vector2.Zero, new Vector2(30, 30), symbolColor, 15, _inkColor, 2);
        icon.AddChild(CreateTextLabel(symbol, 16, Colors.Black, new Vector2(0, 3), new Vector2(30, 24), HorizontalAlignment.Center));
        item.AddChild(icon);

        var status = new Label();
        status.Text = statusText;
        status.AddThemeFontSizeOverride("font_size", 18);
        status.AddThemeColorOverride("font_color", Colors.Black);
        status.VerticalAlignment = VerticalAlignment.Center;
        item.AddChild(status);
        return item;
    }

    private Vector2 GetScaledHexClusterSize(float positionScale, float outerSide)
    {
        var outerSize = GetHexBounds(outerSide);
        var maxX = 0.0f;
        var maxY = 0.0f;
        foreach (var tile in _hexTiles)
        {
            var scaledPos = tile.Position * positionScale;
            maxX = Mathf.Max(maxX, scaledPos.X + outerSize.X);
            maxY = Mathf.Max(maxY, scaledPos.Y + outerSize.Y);
        }
        return new Vector2(maxX, maxY);
    }

    private Control CreateMiniHexTile(
        HexTileData tile,
        Vector2 position,
        float outerSide,
        float innerSide,
        float overlayOuterSide,
        float overlayInnerSide,
        bool isConflict
    )
    {
        var wrapper = new Control();
        wrapper.Position = position;
        var outerSize = GetHexBounds(outerSide);
        wrapper.Size = outerSize;
        var center = outerSize / 2.0f;

        if (isConflict)
        {
            wrapper.AddChild(CreateHexPolygon(outerSide + 7.0f, center, Color.FromHtml("#EEF55D")));
            wrapper.AddChild(CreateHexPolygon(outerSide + 3.0f, center, Color.FromHtml("#E2C54D")));
            wrapper.AddChild(CreateHexPolygon(outerSide, center, _inkColor));
            wrapper.AddChild(CreateHexPolygon(innerSide, center, Color.FromHtml("#F82D23")));
            return wrapper;
        }

        var baseColor = tile.Base == HexBase.Wilds ? _wildsColor : _wastedColor;
        wrapper.AddChild(CreateHexPolygon(outerSide, center, _inkColor));
        wrapper.AddChild(CreateHexPolygon(innerSide, center, baseColor));

        if (tile.Overlay != OverlayType.None)
        {
            var overlayColor = tile.Overlay == OverlayType.Human ? _humanOverlayColor : _techOverlayColor;
            wrapper.AddChild(CreateHexPolygon(overlayOuterSide, center, _inkColor));
            wrapper.AddChild(CreateHexPolygon(overlayInnerSide, center, overlayColor));
        }

        return wrapper;
    }

    private static Panel CreateInfoPanel(
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
        titleLabel.AddThemeFontSizeOverride("font_size", 22);
        titleLabel.AddThemeColorOverride("font_color", Color.FromHtml("#16222B"));
        layout.AddChild(titleLabel);

        var bodyLabel = new Label();
        bodyLabel.Text = body;
        bodyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        bodyLabel.ClipText = true;
        bodyLabel.SizeFlagsVertical = SizeFlags.ExpandFill;
        bodyLabel.AddThemeFontSizeOverride("font_size", 14);
        bodyLabel.AddThemeColorOverride("font_color", Color.FromHtml("#16222B"));
        layout.AddChild(bodyLabel);

        return panel;
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

    private static Polygon2D CreateHexPolygon(float sideLength, Vector2 center, Color color)
    {
        var polygon = new Polygon2D();
        polygon.Color = color;
        polygon.Polygon = BuildRegularHexPolygon(sideLength, center);
        return polygon;
    }

    private static Vector2 GetHexBounds(float sideLength)
    {
        return new Vector2(sideLength * 2.0f, Mathf.Sqrt(3.0f) * sideLength);
    }

    private static Vector2[] BuildRegularHexPolygon(float sideLength, Vector2 center)
    {
        var halfHeight = Mathf.Sqrt(3.0f) * sideLength * 0.5f;
        var halfSide = sideLength * 0.5f;

        return
        [
            new Vector2(center.X + sideLength, center.Y),
            new Vector2(center.X + halfSide, center.Y + halfHeight),
            new Vector2(center.X - halfSide, center.Y + halfHeight),
            new Vector2(center.X - sideLength, center.Y),
            new Vector2(center.X - halfSide, center.Y - halfHeight),
            new Vector2(center.X + halfSide, center.Y - halfHeight),
        ];
    }

    private static Vector2 Shift(Vector2 position)
    {
        return position + LayoutShift;
    }

    private readonly struct HexTileData(Vector2 position, HexBase @base, OverlayType overlay)
    {
        public Vector2 Position { get; } = position;
        public HexBase Base { get; } = @base;
        public OverlayType Overlay { get; } = overlay;
    }

    private enum HexBase
    {
        Wilds,
        Wasted,
    }

    private enum OverlayType
    {
        None,
        Human,
        Tech,
    }
}
