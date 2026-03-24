using Godot;

public partial class HiveBoardView : Node2D
{
    public override void _Ready()
    {
        Configure("Cluster/Tile0", StackView.TileKind.Wilds, null);
        Configure("Cluster/Tile1", StackView.TileKind.Wilds, null);
        Configure("Cluster/Tile2", StackView.TileKind.Wilds, null);
        Configure("Cluster/Tile3", StackView.TileKind.Wasted, null);
        Configure("Cluster/Tile4", StackView.TileKind.Wilds, null);
        Configure("Cluster/Tile5", StackView.TileKind.Wilds, StackView.TileKind.Human);
        Configure("Cluster/Tile6", StackView.TileKind.Wasted, StackView.TileKind.Human);
        Configure("Cluster/Tile7", StackView.TileKind.Wilds, null);
        Configure("Cluster/Tile8", StackView.TileKind.Wilds, null);
        Configure("Cluster/Tile9", StackView.TileKind.Wasted, StackView.TileKind.Technology);
        Configure("Cluster/Tile10", StackView.TileKind.Wasted, StackView.TileKind.Technology);
    }

    private void Configure(string path, StackView.TileKind downType, StackView.TileKind? upType)
    {
        var stack = GetNode<StackView>(path);
        stack.ConfigureTileStack(downType, upType, false);
    }
}
