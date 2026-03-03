using Godot;

public partial class MainUI : Control
{
    private const int TotalTurns = 6;
    private const int TotalResourceCells = 25;
    private const int ConflictCells = 3;

    private readonly Color _inkColor = Color.FromHtml("#2B2726");
    private readonly Color _textColor = Color.FromHtml("#16222B");
    private readonly Color _mutedTextColor = Color.FromHtml("#4B5E69");
    private readonly Color _avatarFillColor = Color.FromHtml("#EAE6E0");
    private readonly Color _passBubbleColor = Color.FromHtml("#D8D3CB");
    private readonly Color _passTextColor = Color.FromHtml("#61F41E");
    private readonly Color _iconFillColor = Color.FromHtml("#DFD7CE");
    private readonly Color _nationFillColor = Color.FromHtml("#F0B54B");
    private readonly Color _trackEmptyColor = Color.FromHtml("#D9D9D9");
    private readonly Color _trackConflictColor = Color.FromHtml("#D46F6F");
    private readonly Color _wildsColor = Color.FromHtml("#6CE575");
    private readonly Color _wastedColor = Color.FromHtml("#D07D29");
    private readonly Color _humanOverlayColor = Color.FromHtml("#C92CC1");
    private readonly Color _techOverlayColor = Color.FromHtml("#3D29ED");

    private readonly PlayerData[] _players =
    {
        new PlayerData(1, "0.0%", true),
        new PlayerData(2, "66.7%", true),
        new PlayerData(3, "10.3%", false),
        new PlayerData(4, "90.7%", true),
        new PlayerData(5, "33.3%", false),
    };

    private readonly HexTileData[] _hexTiles =
    {
        new HexTileData(new Vector2(20, 0),  HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(390, 0), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(760, 0), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(205, 110), HexBase.Wasted, OverlayType.None),
        new HexTileData(new Vector2(575, 110), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(390, 220), HexBase.Wilds, OverlayType.Human),
        new HexTileData(new Vector2(575, 330), HexBase.Wasted, OverlayType.Human),
        new HexTileData(new Vector2(205, 330), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(20, 440), HexBase.Wilds, OverlayType.None),
        new HexTileData(new Vector2(390, 440), HexBase.Wasted, OverlayType.Tech),
        new HexTileData(new Vector2(760, 440), HexBase.Wasted, OverlayType.Tech),
    };

    public override void _Ready()
    {
        var canvas = GetNode<Control>("LayoutCanvas");
        BuildPlayerColumn(canvas);
        BuildNationLevel(canvas);
        BuildTurnDots(canvas, 0);
        BuildHexCluster(canvas);
        BuildResourceTracks(canvas);
        BuildRightPanels(canvas);
    }

    private void BuildPlayerColumn(Control parent)
    {
        var startY = 38.0f;
        var spacing = 150.0f;

        for (int index = 0; index < _players.Length; index++)
        {
            var block = CreatePlayerBlock(_players[index]);
            block.Position = new Vector2(34, startY + spacing * index);
            parent.AddChild(block);
        }
    }

    private Control CreatePlayerBlock(PlayerData player)
    {
        var block = new Control();
        block.Size = new Vector2(180, 122);

        var avatar = CreateRoundPanel(new Vector2(0, 0), new Vector2(88, 88), _avatarFillColor, 44, _inkColor, 2);
        block.AddChild(avatar);
        block.AddChild(CreateTextLabel($"P{player.Slot}", 22, _textColor, new Vector2(0, 26), new Vector2(88, 30), HorizontalAlignment.Center));

        if (player.IsPassing)
        {
            block.AddChild(CreateStatusBubble(new Vector2(96, 2), "PASS"));
        }

        block.AddChild(CreateTextLabel(player.IndividualProcess, 16, _textColor, new Vector2(0, 92), new Vector2(88, 22), HorizontalAlignment.Center));
        return block;
    }

    private void BuildNationLevel(Control parent)
    {
        var wrapper = new Control();
        wrapper.Position = new Vector2(52, 784);
        wrapper.Size = new Vector2(112, 140);

        var outline = new Polygon2D();
        outline.Color = _inkColor;
        outline.Polygon = BuildShieldPolygon(new Vector2(112, 140), Vector2.Zero);
        wrapper.AddChild(outline);

        var fill = new Polygon2D();
        fill.Color = _nationFillColor;
        fill.Polygon = BuildShieldPolygon(new Vector2(102, 130), new Vector2(5, 5));
        wrapper.AddChild(fill);

        wrapper.AddChild(CreateTextLabel("10", 58, Colors.Black, new Vector2(6, 18), new Vector2(100, 64), HorizontalAlignment.Center));
        wrapper.AddChild(CreateTextLabel("nation_level", 10, _mutedTextColor, new Vector2(0, 112), new Vector2(112, 18), HorizontalAlignment.Center));

        parent.AddChild(wrapper);
    }

    private void BuildTurnDots(Control parent, int completedTurns)
    {
        var start = new Vector2(36, 952);
        var dotSize = new Vector2(38, 38);
        var gapX = 48.0f;
        var gapY = 52.0f;

        for (int index = 0; index < TotalTurns; index++)
        {
            var row = index / 3;
            var column = index % 3;
            var fillColor = index < completedTurns ? _techOverlayColor : Color.FromHtml("#ECEAE7");
            parent.AddChild(
                CreateRoundPanel(
                    new Vector2(start.X + gapX * column, start.Y + gapY * row),
                    dotSize,
                    fillColor,
                    19,
                    _inkColor,
                    2
                )
            );
        }
    }

    private void BuildHexCluster(Control parent)
    {
        var cluster = new Control();
        cluster.Position = new Vector2(242, 68);
        cluster.Size = new Vector2(1020, 670);

        foreach (var tile in _hexTiles)
        {
            cluster.AddChild(CreateHexTile(tile));
        }

        parent.AddChild(cluster);
    }

    private Control CreateHexTile(HexTileData tile)
    {
        var wrapper = new Control();
        wrapper.Position = tile.Position;
        wrapper.Size = new Vector2(220, 208);

        var baseColor = tile.Base == HexBase.Wilds ? _wildsColor : _wastedColor;
        wrapper.AddChild(CreateHexPolygon(new Vector2(220, 208), Vector2.Zero, _inkColor));
        wrapper.AddChild(CreateHexPolygon(new Vector2(210, 198), new Vector2(5, 5), baseColor));

        if (tile.Overlay != OverlayType.None)
        {
            var overlayColor = tile.Overlay == OverlayType.Human ? _humanOverlayColor : _techOverlayColor;
            wrapper.AddChild(CreateHexPolygon(new Vector2(136, 130), new Vector2(42, 39), _inkColor));
            wrapper.AddChild(CreateHexPolygon(new Vector2(126, 120), new Vector2(47, 44), overlayColor));
        }

        return wrapper;
    }

    private void BuildResourceTracks(Control parent)
    {
        parent.AddChild(CreateTrackRow(new Vector2(205, 800), "H", "human_resources", 13, Color.FromHtml("#F4F4F4")));
        parent.AddChild(CreateTrackRow(new Vector2(205, 888), "T", "technology_resources", 10, Color.FromHtml("#F4F4F4")));
        parent.AddChild(CreateTrackRow(new Vector2(205, 976), "E", "environment_resources", 8, Color.FromHtml("#F4F4F4")));
    }

    private Control CreateTrackRow(Vector2 position, string iconText, string labelText, int filledCells, Color filledColor)
    {
        var row = new Control();
        row.Position = position;
        row.Size = new Vector2(1010, 64);

        var iconBox = CreateRoundedPanel(Vector2.Zero, new Vector2(72, 72), _iconFillColor, 18);
        row.AddChild(iconBox);
        row.AddChild(CreateTextLabel(iconText, 28, Colors.Black, new Vector2(0, 18), new Vector2(72, 34), HorizontalAlignment.Center));
        row.AddChild(CreateTextLabel(labelText, 11, _mutedTextColor, new Vector2(84, -16), new Vector2(220, 16), HorizontalAlignment.Left));

        var trackStartX = 102.0f;
        var cellWidth = 28.0f;
        var cellHeight = 46.0f;
        var gap = 8.0f;

        for (int index = 0; index < TotalResourceCells; index++)
        {
            var state = ResolveResourceCellState(index, filledCells);
            var cellColor = state switch
            {
                CellState.Filled => filledColor,
                CellState.Conflict => _trackConflictColor,
                _ => _trackEmptyColor,
            };

            var radius = index == 0 || index == TotalResourceCells - 1 ? 14 : 4;
            var cell = CreateRoundedPanel(
                new Vector2(trackStartX + index * (cellWidth + gap), 8),
                new Vector2(cellWidth, cellHeight),
                cellColor,
                radius,
                _inkColor,
                2
            );
            row.AddChild(cell);
        }

        return row;
    }

    private CellState ResolveResourceCellState(int index, int filledCells)
    {
        if (index >= TotalResourceCells - ConflictCells)
        {
            return CellState.Conflict;
        }

        var maxUsableCells = TotalResourceCells - ConflictCells;
        return index < Mathf.Min(filledCells, maxUsableCells) ? CellState.Filled : CellState.Empty;
    }

    private void BuildRightPanels(Control parent)
    {
        parent.AddChild(
            CreateInfoPanel(
                new Vector2(1305, 34),
                new Vector2(500, 232),
                "major_goal",
                "Shared objective for every player:\nStabilize the board, raise nation level, and stop conflict from shrinking the usable tracks.",
                Color.FromHtml("#D7D7D7"),
                0
            )
        );

        parent.AddChild(
            CreateInfoPanel(
                new Vector2(1305, 296),
                new Vector2(500, 292),
                "information_panel",
                "Key state summary:\n- 5 players on the left\n- 11 map hexes in the center\n- 3 resource tracks at the bottom",
                Color.FromHtml("#CFCFCF"),
                30
            )
        );

        parent.AddChild(
            CreateInfoPanel(
                new Vector2(1305, 618),
                new Vector2(500, 350),
                "chat_log",
                "[P1] Secure the wilds.\n[P3] Human build next turn.\n[P5] Conflict blocks the final cells.",
                Color.FromHtml("#F1F1F1"),
                0
            )
        );

        var inputPanel = CreateRoundedPanel(new Vector2(1305, 986), new Vector2(500, 56), Color.FromHtml("#F1F1F1"), 16);
        parent.AddChild(inputPanel);
        parent.AddChild(CreateTextLabel("chat_input", 14, _mutedTextColor, new Vector2(1323, 952), new Vector2(160, 22), HorizontalAlignment.Left));

        var lineEdit = new LineEdit();
        lineEdit.Position = new Vector2(1323, 999);
        lineEdit.Size = new Vector2(464, 30);
        lineEdit.PlaceholderText = "Type a message...";
        parent.AddChild(lineEdit);
    }

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
        panel.AddChild(CreateTextLabel(title, 16, _mutedTextColor, new Vector2(18, 16), new Vector2(size.X - 36, 22), HorizontalAlignment.Left));

        var bodyLabel = CreateTextLabel(body, 15, _textColor, new Vector2(18, 48), new Vector2(size.X - 36, size.Y - 64), HorizontalAlignment.Left);
        bodyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        panel.AddChild(bodyLabel);
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

    private Polygon2D CreateHexPolygon(Vector2 size, Vector2 offset, Color color)
    {
        var polygon = new Polygon2D();
        polygon.Color = color;
        polygon.Polygon = BuildFlatHexPolygon(size, offset);
        return polygon;
    }

    private Vector2[] BuildFlatHexPolygon(Vector2 size, Vector2 offset)
    {
        return new[]
        {
            new Vector2(offset.X + size.X * 0.25f, offset.Y),
            new Vector2(offset.X + size.X * 0.75f, offset.Y),
            new Vector2(offset.X + size.X, offset.Y + size.Y * 0.5f),
            new Vector2(offset.X + size.X * 0.75f, offset.Y + size.Y),
            new Vector2(offset.X + size.X * 0.25f, offset.Y + size.Y),
            new Vector2(offset.X, offset.Y + size.Y * 0.5f),
        };
    }

    private Vector2[] BuildShieldPolygon(Vector2 size, Vector2 offset)
    {
        return new[]
        {
            new Vector2(offset.X + size.X * 0.12f, offset.Y),
            new Vector2(offset.X + size.X * 0.88f, offset.Y),
            new Vector2(offset.X + size.X, offset.Y + size.Y * 0.22f),
            new Vector2(offset.X + size.X, offset.Y + size.Y * 0.64f),
            new Vector2(offset.X + size.X * 0.5f, offset.Y + size.Y),
            new Vector2(offset.X, offset.Y + size.Y * 0.64f),
            new Vector2(offset.X, offset.Y + size.Y * 0.22f),
        };
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

    private readonly struct PlayerData
    {
        public PlayerData(int slot, string individualProcess, bool isPassing)
        {
            Slot = slot;
            IndividualProcess = individualProcess;
            IsPassing = isPassing;
        }

        public int Slot { get; }
        public string IndividualProcess { get; }
        public bool IsPassing { get; }
    }

    private readonly struct HexTileData
    {
        public HexTileData(Vector2 position, HexBase @base, OverlayType overlay)
        {
            Position = position;
            Base = @base;
            Overlay = overlay;
        }

        public Vector2 Position { get; }
        public HexBase Base { get; }
        public OverlayType Overlay { get; }
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

    private enum CellState
    {
        Empty,
        Filled,
        Conflict,
    }
}
