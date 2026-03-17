using Godot;

public partial class MainUI
{
    private void BuildRightPanels(Control parent)
    {
        _teamGoalPreviewPanel = CreateInfoPanel(
            Shift(new Vector2(1305, 34)),
            new Vector2(500, 232),
            "Team Goal",
            "Shared objective for every player:\nStabilize the board, raise nation level, and stop conflict from shrinking the usable tracks.",
            Color.FromHtml("#D7D7D7"),
            0
        );
        parent.AddChild(_teamGoalPreviewPanel);

        _teamGoalHitArea = new Button();
        _teamGoalHitArea.Position = _teamGoalPreviewPanel.Position;
        _teamGoalHitArea.Size = _teamGoalPreviewPanel.Size;
        _teamGoalHitArea.Flat = true;
        _teamGoalHitArea.Text = string.Empty;
        _teamGoalHitArea.Modulate = new Color(1, 1, 1, 0);
        _teamGoalHitArea.FocusMode = Control.FocusModeEnum.None;
        _teamGoalHitArea.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        _teamGoalHitArea.ZIndex = 82;
        _teamGoalHitArea.Pressed += ToggleTeamGoalDropdown;
        parent.AddChild(_teamGoalHitArea);

        parent.AddChild(
            CreateInfoPanel(
                Shift(new Vector2(1305, 296)),
                new Vector2(500, 292),
                "Information Panel",
                "Key state summary:\n- 5 players on the left\n- 11 map hexes in the center\n- 3 resource tracks at the bottom",
                Color.FromHtml("#CFCFCF"),
                30
            )
        );

        BuildChatLogPanel(parent);

        var inputPanel = CreateRoundedPanel(Shift(new Vector2(1305, 986)), new Vector2(500, 56), Color.FromHtml("#F1F1F1"), 16);
        parent.AddChild(inputPanel);

        _chatInputLineEdit = new LineEdit();
        _chatInputLineEdit.Position = Shift(new Vector2(1323, 999));
        _chatInputLineEdit.Size = new Vector2(464, 30);
        _chatInputLineEdit.PlaceholderText = "Type a message...";
        _chatInputLineEdit.TextSubmitted += OnChatInputSubmitted;
        parent.AddChild(_chatInputLineEdit);
    }

    private void BuildTeamGoalDropdown(Control parent)
    {
        _teamGoalBackdrop = new Button();
        _teamGoalBackdrop.AnchorRight = 1.0f;
        _teamGoalBackdrop.AnchorBottom = 1.0f;
        _teamGoalBackdrop.GrowHorizontal = Control.GrowDirection.Both;
        _teamGoalBackdrop.GrowVertical = Control.GrowDirection.Both;
        _teamGoalBackdrop.Flat = true;
        _teamGoalBackdrop.Text = string.Empty;
        _teamGoalBackdrop.Modulate = new Color(1, 1, 1, 0);
        _teamGoalBackdrop.FocusMode = Control.FocusModeEnum.None;
        _teamGoalBackdrop.Visible = false;
        _teamGoalBackdrop.ZIndex = 89;
        _teamGoalBackdrop.Pressed += HideTeamGoalDropdown;
        parent.AddChild(_teamGoalBackdrop);

        _teamGoalDropdownPanel = CreateRoundedPanel(
            Shift(new Vector2(1045, 34)),
            new Vector2(760, 900),
            Colors.Transparent,
            0
        );
        _teamGoalDropdownPanel.Visible = false;
        _teamGoalDropdownPanel.ZIndex = 90;
        _teamGoalDropdownPanel.ClipContents = false;
        parent.AddChild(_teamGoalDropdownPanel);

        var sections = new VBoxContainer();
        sections.AnchorRight = 1.0f;
        sections.AnchorBottom = 1.0f;
        sections.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        sections.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        sections.AddThemeConstantOverride("separation", 14);
        _teamGoalDropdownPanel.AddChild(sections);

        sections.AddChild(CreateTeamGoalDescriptionSection(new Vector2(760, 300)));
        sections.AddChild(CreateTeamGoalMiniGridSection(new Vector2(760, 360)));
        sections.AddChild(CreateTeamGoalConditionSection(new Vector2(760, 170)));
    }

    private void ToggleTeamGoalDropdown()
    {
        if (_teamGoalDropdownPanel.Visible)
        {
            HideTeamGoalDropdown();
        }
        else
        {
            ShowTeamGoalDropdown();
        }
    }

    private void ShowTeamGoalDropdown()
    {
        _teamGoalBackdrop.Visible = true;
        _teamGoalDropdownPanel.Visible = true;
    }

    private void HideTeamGoalDropdown()
    {
        _teamGoalDropdownPanel.Visible = false;
        _teamGoalBackdrop.Visible = false;
    }

    private Panel CreateTeamGoalDescriptionSection(Vector2 size)
    {
        var section = CreateRoundedPanel(Vector2.Zero, size, Color.FromHtml("#EDEDED"), 44, _inkColor, 3);
        section.CustomMinimumSize = size;
        section.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
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

        var flavor = CreateTextLabel(
            "We sought dynamic balance in nature and artifice.",
            18,
            Color.FromHtml("#666666"),
            new Vector2(30, 238),
            new Vector2(size.X - 60, 44),
            HorizontalAlignment.Center
        );
        flavor.AddThemeFontSizeOverride("font_size", 18);
        section.AddChild(flavor);

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

    private Panel CreateTeamGoalMiniGridSection(Vector2 size)
    {
        var section = CreateRoundedPanel(Vector2.Zero, size, Color.FromHtml("#E9E9E9"), 44, _inkColor, 3);
        section.CustomMinimumSize = size;
        section.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
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

    private Panel CreateTeamGoalConditionSection(Vector2 size)
    {
        var section = CreateRoundedPanel(Vector2.Zero, size, Color.FromHtml("#EFEFEF"), 40, _inkColor, 3);
        section.CustomMinimumSize = size;
        section.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
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

        var icon = CreateRoundPanel(Vector2.Zero, new Vector2(30, 30), symbolColor, 15, _inkColor, 2);
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
}
