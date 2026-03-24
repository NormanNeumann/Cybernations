using Godot;

public partial class HiveBoardView : Node2D
{
    public override void _Ready()
    {
        Configure("Cluster/Tile0", StackView.StackBaseKind.Wilds, StackView.StackOverlayKind.None);
        Configure("Cluster/Tile1", StackView.StackBaseKind.Wilds, StackView.StackOverlayKind.None);
        Configure("Cluster/Tile2", StackView.StackBaseKind.Wilds, StackView.StackOverlayKind.None);
        Configure("Cluster/Tile3", StackView.StackBaseKind.Wasted, StackView.StackOverlayKind.None);
        Configure("Cluster/Tile4", StackView.StackBaseKind.Wilds, StackView.StackOverlayKind.None);
        Configure("Cluster/Tile5", StackView.StackBaseKind.Wilds, StackView.StackOverlayKind.Human);
        Configure("Cluster/Tile6", StackView.StackBaseKind.Wasted, StackView.StackOverlayKind.Human);
        Configure("Cluster/Tile7", StackView.StackBaseKind.Wilds, StackView.StackOverlayKind.None);
        Configure("Cluster/Tile8", StackView.StackBaseKind.Wilds, StackView.StackOverlayKind.None);
        Configure("Cluster/Tile9", StackView.StackBaseKind.Wasted, StackView.StackOverlayKind.Tech);
        Configure("Cluster/Tile10", StackView.StackBaseKind.Wasted, StackView.StackOverlayKind.Tech);
    }

    private void Configure(string path, StackView.StackBaseKind baseKind, StackView.StackOverlayKind overlayKind)
    {
        var stack = GetNode<StackView>(path);
        stack.Configure(baseKind, overlayKind, false);
    }
}
