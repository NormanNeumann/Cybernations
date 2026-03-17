using Godot;

public partial class MainUI
{
    private void BuildPlayerColumn(Control parent)
    {
        var startY = 38.0f;
        var spacing = 150.0f;

        for (int index = 0; index < _players.Length; index++)
        {
            var block = CreatePlayerBlock(_players[index]);
            block.Position = Shift(new Vector2(34, startY + spacing * index));
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

        var avatarClickArea = new Button();
        avatarClickArea.Position = Vector2.Zero;
        avatarClickArea.Size = new Vector2(88, 88);
        avatarClickArea.Flat = true;
        avatarClickArea.Text = string.Empty;
        avatarClickArea.Modulate = new Color(1, 1, 1, 0);
        avatarClickArea.FocusMode = Control.FocusModeEnum.None;
        avatarClickArea.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        avatarClickArea.Pressed += () => ShowPlayerDetail(player, block.Position + new Vector2(102, 0));
        block.AddChild(avatarClickArea);

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
        wrapper.Position = Shift(new Vector2(52, 784));
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

        parent.AddChild(wrapper);
    }

    private void BuildTurnDots(Control parent, int completedTurns)
    {
        var start = Shift(new Vector2(36, 952));
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
}
