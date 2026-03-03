using Godot;
using System;

public partial class MainUI : Control
{
    private const int TotalTurns = 6;
    private const int TotalResourceCells = 12;
    private const int ConflictCells = 3;

    private readonly Color _textColor = Color.FromHtml("#17364B");
    private readonly Color _mutedTextColor = Color.FromHtml("#546D7E");
    private readonly Color _panelColor = Color.FromHtml("#F8FBFD");
    private readonly Color _panelBorderColor = Color.FromHtml("#A9BFCC");
    private readonly Color _leftAccent = Color.FromHtml("#D9EBF5");
    private readonly Color _trackConflictColor = Color.FromHtml("#C94C4C");
    private readonly Color _trackEmptyColor = Color.FromHtml("#E8EFF3");
    private readonly Color _wildsColor = Color.FromHtml("#00FF73");
    private readonly Color _wastedColor = Color.FromHtml("#D87300");
    private readonly Color _humanOverlayColor = Color.FromHtml("#E200C4");
    private readonly Color _techOverlayColor = Color.FromHtml("#3628FF");

    private readonly PlayerData[] _players =
    {
        new PlayerData(1, "P1", 28, 2, 1),
        new PlayerData(2, "P2", 41, 4, 2),
        new PlayerData(3, "P3", 57, 5, 3),
        new PlayerData(4, "P4", 66, 7, 4),
        new PlayerData(5, "P5", 81, 9, 5),
    };

    public override void _Ready()
    {
        BuildPlayers();
        BuildCenterBoard();
        BuildRightPanels();
    }

    private void BuildPlayers()
    {
        var leftColumn = GetNode<VBoxContainer>("RootMargin/MainLayout/LeftColumn");
        leftColumn.AddChild(CreateSectionTitle("Players"));

        foreach (var player in _players)
        {
            var card = CreateStyledPanel(_panelColor, _panelBorderColor, 18);
            card.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

            var margin = new MarginContainer();
            ApplyMargins(margin, 14, 14, 14, 14);
            card.AddChild(margin);

            var content = new VBoxContainer();
            content.AddThemeConstantOverride("separation", 6);
            margin.AddChild(content);

            content.AddChild(CreateTextLabel($"Player {player.Slot}", 16, _textColor, HorizontalAlignment.Center));
            content.AddChild(CreateCentered(CreateCircleBadge(player.AvatarText, 82, _leftAccent, _panelBorderColor, _textColor, 20)));
            content.AddChild(CreateSmallCaption("individual_process"));
            content.AddChild(CreateTextLabel(player.IndividualProcess.ToString(), 24, _textColor, HorizontalAlignment.Center));
            content.AddChild(CreateSmallCaption("nation_level"));
            content.AddChild(CreateCentered(CreateNationLevelRow(player.NationLevel)));
            content.AddChild(CreateCentered(CreateTurnRow(player.CompletedTurns)));

            leftColumn.AddChild(card);
        }
    }

    private void BuildCenterBoard()
    {
        var centerColumn = GetNode<VBoxContainer>("RootMargin/MainLayout/CenterColumn");
        centerColumn.AddChild(CreateSectionTitle("Main Board"));

        var boardContent = new VBoxContainer();
        boardContent.AddThemeConstantOverride("separation", 10);

        var legend = CreateBodyLabel(
            "Green = wilds | Orange = wasted | Pink = built_for_human | Blue = built_for_tech",
            14
        );
        boardContent.AddChild(legend);
        boardContent.AddChild(BuildHexGrid());

        var boardPanel = CreateSectionCard("central_field", boardContent, 580);
        boardPanel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        boardPanel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        centerColumn.AddChild(boardPanel);

        var relationsContent = new VBoxContainer();
        relationsContent.AddThemeConstantOverride("separation", 12);
        relationsContent.AddChild(
            CreateBodyLabel(
                $"Conflict = {ConflictCells}. The right-most {ConflictCells} cells are locked on every track and reduce the cap for all resources.",
                14
            )
        );

        var tracks = new VBoxContainer();
        tracks.AddThemeConstantOverride("separation", 10);
        tracks.AddChild(CreateResourceTrack(new ResourceTrackData("H", "human_resources", 7, Color.FromHtml("#E45B77"))));
        tracks.AddChild(CreateResourceTrack(new ResourceTrackData("T", "technology_resources", 6, Color.FromHtml("#4D8EF0"))));
        tracks.AddChild(CreateResourceTrack(new ResourceTrackData("E", "environment_resources", 5, Color.FromHtml("#4CBF85"))));
        relationsContent.AddChild(tracks);

        var relationsPanel = CreateSectionCard("relations_track", relationsContent, 250);
        relationsPanel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        centerColumn.AddChild(relationsPanel);
    }

    private void BuildRightPanels()
    {
        var rightColumn = GetNode<VBoxContainer>("RootMargin/MainLayout/RightColumn");
        rightColumn.AddChild(CreateSectionTitle("Info Windows"));

        var majorGoal = CreateBodyLabel(
            "All players share one major goal:\n" +
            "- Convert the map into a stable nation state.\n" +
            "- Balance human, technology, and environment growth.\n" +
            "- Keep conflict from collapsing the usable resource cap.",
            14
        );
        rightColumn.AddChild(CreateSectionCard("major_goal", majorGoal, 170));

        var infoPanel = CreateBodyLabel(
            "Key information to surface during play:\n" +
            "- Which player is leading in individual_process.\n" +
            "- Which nation_level is closest to 10.\n" +
            "- Which resource track is about to hit the conflict wall.",
            14
        );
        rightColumn.AddChild(CreateSectionCard("information_panel", infoPanel, 190));

        var chatLog = CreateBodyLabel(
            "[P1] We should secure the upper wilds tile.\n" +
            "[P3] Human build on the center lane next turn.\n" +
            "[P5] Conflict already blocks the last three cells.",
            14
        );
        var chatLogPanel = CreateSectionCard("chat_log", chatLog, 220);
        chatLogPanel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        rightColumn.AddChild(chatLogPanel);

        var chatInputContent = new VBoxContainer();
        chatInputContent.AddThemeConstantOverride("separation", 8);
        chatInputContent.AddChild(CreateBodyLabel("Use this box for player discussion and turn planning.", 13));

        var input = new TextEdit();
        input.CustomMinimumSize = new Vector2(0, 110);
        input.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        chatInputContent.AddChild(input);

        rightColumn.AddChild(CreateSectionCard("chat_input", chatInputContent, 180));
    }

    private Control BuildHexGrid()
    {
        var grid = new Control();
        grid.CustomMinimumSize = new Vector2(900, 470);
        grid.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        grid.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

        var tiles =
            new[]
            {
                new HexTileData(new Vector2(120, 10), "wilds", _wildsColor, false, string.Empty, Colors.Transparent),
                new HexTileData(new Vector2(300, 10), "wasted", _wastedColor, true, "built_for_tech", _techOverlayColor),
                new HexTileData(new Vector2(480, 10), "wilds", _wildsColor, true, "built_for_human", _humanOverlayColor),
                new HexTileData(new Vector2(30, 150), "wasted", _wastedColor, false, string.Empty, Colors.Transparent),
                new HexTileData(new Vector2(210, 150), "wilds", _wildsColor, true, "built_for_human", _humanOverlayColor),
                new HexTileData(new Vector2(390, 150), "wilds", _wildsColor, false, string.Empty, Colors.Transparent),
                new HexTileData(new Vector2(570, 150), "wasted", _wastedColor, true, "built_for_tech", _techOverlayColor),
                new HexTileData(new Vector2(30, 290), "wilds", _wildsColor, false, string.Empty, Colors.Transparent),
                new HexTileData(new Vector2(210, 290), "wasted", _wastedColor, true, "built_for_human", _humanOverlayColor),
                new HexTileData(new Vector2(390, 290), "wilds", _wildsColor, true, "built_for_tech", _techOverlayColor),
                new HexTileData(new Vector2(570, 290), "wasted", _wastedColor, false, string.Empty, Colors.Transparent),
            };

        foreach (var tile in tiles)
        {
            grid.AddChild(CreateHexTile(tile));
        }

        return grid;
    }

    private Control CreateHexTile(HexTileData tile)
    {
        var wrapper = new Control();
        wrapper.Position = tile.Position;
        wrapper.CustomMinimumSize = new Vector2(160, 140);

        var baseHex = new Polygon2D();
        baseHex.Polygon = BuildHexPoints(new Vector2(80, 54), 52);
        baseHex.Color = tile.BaseColor;
        wrapper.AddChild(baseHex);

        if (tile.HasOverlay)
        {
            var overlayHex = new Polygon2D();
            overlayHex.Polygon = BuildHexPoints(new Vector2(80, 54), 28);
            overlayHex.Color = tile.OverlayColor;
            wrapper.AddChild(overlayHex);
        }

        var labels = new VBoxContainer();
        labels.Position = new Vector2(10, 98);
        labels.CustomMinimumSize = new Vector2(140, 36);
        labels.AddThemeConstantOverride("separation", 2);
        labels.AddChild(CreateTextLabel(tile.BaseLabel, 13, _textColor, HorizontalAlignment.Center));
        labels.AddChild(
            CreateTextLabel(
                tile.HasOverlay ? tile.OverlayLabel : "base only",
                11,
                _mutedTextColor,
                HorizontalAlignment.Center
            )
        );
        wrapper.AddChild(labels);

        return wrapper;
    }

    private HBoxContainer CreateResourceTrack(ResourceTrackData data)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 8);
        row.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

        row.AddChild(CreateCircleBadge(data.ShortCode, 28, _leftAccent, _panelBorderColor, _textColor, 14));

        var name = CreateTextLabel(data.Name, 14, _textColor, HorizontalAlignment.Left);
        name.CustomMinimumSize = new Vector2(170, 0);
        row.AddChild(name);

        var spacer = new Control();
        spacer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(spacer);

        for (int index = 0; index < TotalResourceCells; index++)
        {
            row.AddChild(CreateResourceCell(ResolveResourceCellState(index, data.FilledCells), data.FilledColor));
        }

        return row;
    }

    private Panel CreateResourceCell(CellState state, Color filledColor)
    {
        var cell = new Panel();
        cell.CustomMinimumSize = new Vector2(20, 28);

        var style = new StyleBoxFlat();
        style.CornerRadiusTopLeft = 6;
        style.CornerRadiusTopRight = 6;
        style.CornerRadiusBottomLeft = 6;
        style.CornerRadiusBottomRight = 6;
        style.BorderWidthLeft = 1;
        style.BorderWidthTop = 1;
        style.BorderWidthRight = 1;
        style.BorderWidthBottom = 1;

        switch (state)
        {
            case CellState.Filled:
                style.BgColor = filledColor;
                style.BorderColor = filledColor.Darkened(0.25f);
                break;
            case CellState.Conflict:
                style.BgColor = _trackConflictColor;
                style.BorderColor = _trackConflictColor.Darkened(0.25f);
                break;
            default:
                style.BgColor = _trackEmptyColor;
                style.BorderColor = _panelBorderColor;
                break;
        }

        cell.AddThemeStyleboxOverride("panel", style);
        return cell;
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

    private HBoxContainer CreateNationLevelRow(int nationLevel)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 6);
        row.AddChild(CreateCircleBadge("N", 26, _textColor, _textColor, Colors.White, 12));
        row.AddChild(CreateTextLabel($"{nationLevel} / 10", 14, _textColor, HorizontalAlignment.Left));
        return row;
    }

    private HBoxContainer CreateTurnRow(int completedTurns)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 5);

        for (int turn = 0; turn < TotalTurns; turn++)
        {
            var dot = new Panel();
            dot.CustomMinimumSize = new Vector2(16, 16);

            var style = new StyleBoxFlat();
            style.CornerRadiusTopLeft = 8;
            style.CornerRadiusTopRight = 8;
            style.CornerRadiusBottomLeft = 8;
            style.CornerRadiusBottomRight = 8;
            style.BorderWidthLeft = 1;
            style.BorderWidthTop = 1;
            style.BorderWidthRight = 1;
            style.BorderWidthBottom = 1;
            style.BorderColor = _panelBorderColor;
            style.BgColor = turn < completedTurns ? _techOverlayColor : Color.FromHtml("#FFFFFF");
            dot.AddThemeStyleboxOverride("panel", style);

            row.AddChild(dot);
        }

        return row;
    }

    private PanelContainer CreateCircleBadge(
        string text,
        int diameter,
        Color fillColor,
        Color borderColor,
        Color textColor,
        int fontSize
    )
    {
        var badge = new PanelContainer();
        badge.CustomMinimumSize = new Vector2(diameter, diameter);

        var style = new StyleBoxFlat();
        style.BgColor = fillColor;
        style.BorderColor = borderColor;
        style.BorderWidthLeft = 2;
        style.BorderWidthTop = 2;
        style.BorderWidthRight = 2;
        style.BorderWidthBottom = 2;
        style.CornerRadiusTopLeft = diameter / 2;
        style.CornerRadiusTopRight = diameter / 2;
        style.CornerRadiusBottomLeft = diameter / 2;
        style.CornerRadiusBottomRight = diameter / 2;
        badge.AddThemeStyleboxOverride("panel", style);

        var center = new CenterContainer();
        center.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        center.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        badge.AddChild(center);

        center.AddChild(CreateTextLabel(text, fontSize, textColor, HorizontalAlignment.Center));
        return badge;
    }

    private PanelContainer CreateSectionCard(string title, Control content, int minimumHeight)
    {
        var panel = CreateStyledPanel(_panelColor, _panelBorderColor, 18);
        panel.CustomMinimumSize = new Vector2(0, minimumHeight);

        var margin = new MarginContainer();
        ApplyMargins(margin, 18, 16, 18, 16);
        panel.AddChild(margin);

        var layout = new VBoxContainer();
        layout.AddThemeConstantOverride("separation", 12);
        margin.AddChild(layout);

        layout.AddChild(CreateTextLabel(title, 20, _textColor, HorizontalAlignment.Left));
        content.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        content.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        layout.AddChild(content);

        return panel;
    }

    private PanelContainer CreateStyledPanel(Color backgroundColor, Color borderColor, int radius)
    {
        var panel = new PanelContainer();

        var style = new StyleBoxFlat();
        style.BgColor = backgroundColor;
        style.BorderColor = borderColor;
        style.BorderWidthLeft = 2;
        style.BorderWidthTop = 2;
        style.BorderWidthRight = 2;
        style.BorderWidthBottom = 2;
        style.CornerRadiusTopLeft = radius;
        style.CornerRadiusTopRight = radius;
        style.CornerRadiusBottomLeft = radius;
        style.CornerRadiusBottomRight = radius;

        panel.AddThemeStyleboxOverride("panel", style);
        return panel;
    }

    private Label CreateSectionTitle(string text)
    {
        return CreateTextLabel(text, 28, _textColor, HorizontalAlignment.Left);
    }

    private Label CreateSmallCaption(string text)
    {
        return CreateTextLabel(text, 11, _mutedTextColor, HorizontalAlignment.Center);
    }

    private Label CreateBodyLabel(string text, int fontSize)
    {
        var label = CreateTextLabel(text, fontSize, _textColor, HorizontalAlignment.Left);
        label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        return label;
    }

    private Label CreateTextLabel(string text, int fontSize, Color color, HorizontalAlignment alignment)
    {
        var label = new Label();
        label.Text = text;
        label.HorizontalAlignment = alignment;
        label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", color);
        return label;
    }

    private CenterContainer CreateCentered(Control child)
    {
        var center = new CenterContainer();
        center.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        center.AddChild(child);
        return center;
    }

    private void ApplyMargins(MarginContainer container, int left, int top, int right, int bottom)
    {
        container.AddThemeConstantOverride("margin_left", left);
        container.AddThemeConstantOverride("margin_top", top);
        container.AddThemeConstantOverride("margin_right", right);
        container.AddThemeConstantOverride("margin_bottom", bottom);
    }

    private Vector2[] BuildHexPoints(Vector2 center, float radius)
    {
        var points = new Vector2[6];
        for (int index = 0; index < points.Length; index++)
        {
            var angle = Mathf.DegToRad(60 * index - 30);
            points[index] = new Vector2(center.X + radius * Mathf.Cos(angle), center.Y + radius * Mathf.Sin(angle));
        }

        return points;
    }

    private readonly struct PlayerData
    {
        public PlayerData(int slot, string avatarText, int individualProcess, int nationLevel, int completedTurns)
        {
            Slot = slot;
            AvatarText = avatarText;
            IndividualProcess = individualProcess;
            NationLevel = nationLevel;
            CompletedTurns = completedTurns;
        }

        public int Slot { get; }
        public string AvatarText { get; }
        public int IndividualProcess { get; }
        public int NationLevel { get; }
        public int CompletedTurns { get; }
    }

    private readonly struct HexTileData
    {
        public HexTileData(
            Vector2 position,
            string baseLabel,
            Color baseColor,
            bool hasOverlay,
            string overlayLabel,
            Color overlayColor
        )
        {
            Position = position;
            BaseLabel = baseLabel;
            BaseColor = baseColor;
            HasOverlay = hasOverlay;
            OverlayLabel = overlayLabel;
            OverlayColor = overlayColor;
        }

        public Vector2 Position { get; }
        public string BaseLabel { get; }
        public Color BaseColor { get; }
        public bool HasOverlay { get; }
        public string OverlayLabel { get; }
        public Color OverlayColor { get; }
    }

    private readonly struct ResourceTrackData
    {
        public ResourceTrackData(string shortCode, string name, int filledCells, Color filledColor)
        {
            ShortCode = shortCode;
            Name = name;
            FilledCells = filledCells;
            FilledColor = filledColor;
        }

        public string ShortCode { get; }
        public string Name { get; }
        public int FilledCells { get; }
        public Color FilledColor { get; }
    }

    private enum CellState
    {
        Empty,
        Filled,
        Conflict,
    }
}
