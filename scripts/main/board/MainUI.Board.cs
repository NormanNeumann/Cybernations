using Godot;

public partial class MainUI
{
    private void BuildHexCluster(Control parent)
    {
        var clusterSize = GetHexClusterSize();
        var boardArea = new Rect2(Shift(new Vector2(240, 48)), new Vector2(1040, 690));

        var cluster = new Control();
        cluster.Position = new Vector2(
            boardArea.Position.X + (boardArea.Size.X - clusterSize.X) * 0.5f,
            boardArea.Position.Y + (boardArea.Size.Y - clusterSize.Y) * 0.5f
        );
        cluster.Size = clusterSize;

        foreach (var tile in _hexTiles)
        {
            cluster.AddChild(CreateHexTile(tile));
        }

        parent.AddChild(cluster);
    }

    private Vector2 GetHexClusterSize()
    {
        var outerSize = GetHexBounds(HexOuterSide);
        var maxX = 0.0f;
        var maxY = 0.0f;

        foreach (var tile in _hexTiles)
        {
            maxX = Mathf.Max(maxX, tile.Position.X + outerSize.X);
            maxY = Mathf.Max(maxY, tile.Position.Y + outerSize.Y);
        }

        return new Vector2(maxX, maxY);
    }

    private Control CreateHexTile(HexTileData tile)
    {
        var wrapper = new Control();
        wrapper.Position = tile.Position;
        var outerSize = GetHexBounds(HexOuterSide);
        wrapper.Size = outerSize;

        var baseColor = tile.Base == HexBase.Wilds ? _wildsColor : _wastedColor;
        var center = outerSize / 2.0f;
        wrapper.AddChild(CreateHexPolygon(HexOuterSide, center, _inkColor));
        wrapper.AddChild(CreateHexPolygon(HexInnerSide, center, baseColor));

        if (tile.Overlay != OverlayType.None)
        {
            var overlayColor = tile.Overlay == OverlayType.Human ? _humanOverlayColor : _techOverlayColor;
            wrapper.AddChild(CreateHexPolygon(HexOverlayOuterSide, center, _inkColor));
            wrapper.AddChild(CreateHexPolygon(HexOverlayInnerSide, center, overlayColor));
        }

        return wrapper;
    }

    private void BuildResourceTracks(Control parent)
    {
        parent.AddChild(CreateTrackRow(Shift(new Vector2(205, 800)), "H", 13, Color.FromHtml("#F4F4F4")));
        parent.AddChild(CreateTrackRow(Shift(new Vector2(205, 888)), "T", 10, Color.FromHtml("#F4F4F4")));
        parent.AddChild(CreateTrackRow(Shift(new Vector2(205, 976)), "E", 8, Color.FromHtml("#F4F4F4")));
    }

    private Control CreateTrackRow(Vector2 position, string iconText, int filledCells, Color filledColor)
    {
        var row = new Control();
        row.Position = position;
        row.Size = new Vector2(1010, 64);

        var iconBox = CreateRoundedPanel(Vector2.Zero, new Vector2(72, 72), _iconFillColor, 18);
        row.AddChild(iconBox);
        row.AddChild(CreateTextLabel(iconText, 28, Colors.Black, new Vector2(0, 18), new Vector2(72, 34), HorizontalAlignment.Center));

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

    private Polygon2D CreateHexPolygon(float sideLength, Vector2 center, Color color)
    {
        var polygon = new Polygon2D();
        polygon.Color = color;
        polygon.Polygon = BuildRegularHexPolygon(sideLength, center);
        return polygon;
    }

    private Vector2 GetHexBounds(float sideLength)
    {
        return new Vector2(sideLength * 2.0f, Mathf.Sqrt(3.0f) * sideLength);
    }

    private Vector2[] BuildRegularHexPolygon(float sideLength, Vector2 center)
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
}
