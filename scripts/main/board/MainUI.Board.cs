using Godot;

public partial class MainUI
{
    private static readonly PackedScene StackScene = GD.Load<PackedScene>("res://scenes/stacks/Stack.tscn");

    private void BuildHexCluster(Node2D parent)
    {
        var clusterSize = GetHexClusterSize();
        var boardArea = new Rect2(Shift(new Vector2(240, 48)), new Vector2(1040, 690));

        var cluster = new Node2D();
        cluster.Position = new Vector2(
            boardArea.Position.X + (boardArea.Size.X - clusterSize.X) * 0.5f,
            boardArea.Position.Y + (boardArea.Size.Y - clusterSize.Y) * 0.5f
        );

        foreach (var tile in _hexTiles)
        {
            var stackView = StackScene.Instantiate<StackView>();
            stackView.Position = tile.Position;
            stackView.Configure(
                tile.Base == HexBase.Wilds ? StackView.StackBaseKind.Wilds : StackView.StackBaseKind.Wasted,
                tile.Overlay switch
                {
                    OverlayType.Human => StackView.StackOverlayKind.Human,
                    OverlayType.Tech => StackView.StackOverlayKind.Tech,
                    _ => StackView.StackOverlayKind.None,
                },
                false
            );
            cluster.AddChild(stackView);
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

    private static Vector2 GetHexBounds(float sideLength)
    {
        return new Vector2(sideLength * 2.0f, Mathf.Sqrt(3.0f) * sideLength);
    }
}
