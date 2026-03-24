using System.Collections.Generic;
using Godot;

public partial class HiveBoardView : Node2D
{
    [Export]
    public PackedScene? TileScene { get; set; }

    private Node2D _cluster = null!;
    private readonly Dictionary<int, StackView> _tileViews = [];

    private readonly TilePlacement[] _defaultPlacements =
    [
        new TilePlacement(0, new Vector2(0, 0), StackView.TileKind.Wilds, null),
        new TilePlacement(1, new Vector2(348, 0), StackView.TileKind.Wilds, null),
        new TilePlacement(2, new Vector2(696, 0), StackView.TileKind.Wilds, null),
        new TilePlacement(3, new Vector2(174, 100), StackView.TileKind.Wasted, null),
        new TilePlacement(4, new Vector2(522, 100), StackView.TileKind.Wilds, null),
        new TilePlacement(5, new Vector2(348, 200), StackView.TileKind.Wilds, StackView.TileKind.Human),
        new TilePlacement(6, new Vector2(522, 300), StackView.TileKind.Wasted, StackView.TileKind.Human),
        new TilePlacement(7, new Vector2(174, 300), StackView.TileKind.Wilds, null),
        new TilePlacement(8, new Vector2(0, 400), StackView.TileKind.Wilds, null),
        new TilePlacement(9, new Vector2(348, 400), StackView.TileKind.Wasted, StackView.TileKind.Technology),
        new TilePlacement(10, new Vector2(696, 400), StackView.TileKind.Wasted, StackView.TileKind.Technology),
    ];

    public override void _Ready()
    {
        _cluster = GetNode<Node2D>("Cluster");
        BuildFixedBoard();
    }

    public void BuildDefaultBoard()
    {
        BuildFixedBoard();
    }

    public bool TrySetDownTile(int tileIndex, StackView.TileKind downType, bool conflictHighlight = false)
    {
        if (!TryGetTile(tileIndex, out var tileView))
        {
            return false;
        }

        tileView.ConfigureDownTile(downType, conflictHighlight);
        return true;
    }

    public bool TrySetUpTile(int tileIndex, StackView.TileKind upType)
    {
        if (!TryGetTile(tileIndex, out var tileView))
        {
            return false;
        }

        tileView.ConfigureUpTile(upType);
        return true;
    }

    public bool TryClearUpTile(int tileIndex)
    {
        if (!TryGetTile(tileIndex, out var tileView))
        {
            return false;
        }

        tileView.ClearUpTile();
        return true;
    }

    public bool TrySetConflictHighlight(int tileIndex, bool conflictHighlight)
    {
        if (!TryGetTile(tileIndex, out var tileView))
        {
            return false;
        }

        StackView.TileKind? currentUpType = tileView.HasUpTile ? tileView.UpTileType : null;
        tileView.ConfigureTileStack(
            tileView.DownTileType,
            currentUpType,
            conflictHighlight,
            tileView.DownOuterSide,
            tileView.DownInnerSide,
            tileView.UpOuterSide,
            tileView.UpInnerSide
        );
        return true;
    }

    public bool TryConfigureTile(
        int tileIndex,
        StackView.TileKind downType,
        StackView.TileKind? upType,
        bool conflictHighlight = false
    )
    {
        if (!TryGetTile(tileIndex, out var tileView))
        {
            return false;
        }

        tileView.ConfigureTileStack(downType, upType, conflictHighlight);
        return true;
    }

    public bool TrySetRelationTexture(int tileIndex, int edgeIndex, Texture2D? relationTexture)
    {
        if (!TryGetTile(tileIndex, out var tileView))
        {
            return false;
        }

        tileView.SetRelationTexture(edgeIndex, relationTexture);
        return true;
    }

    public bool TrySetPath(
        int tileIndex,
        int edgeIndex,
        StackView.PathKind pathKind,
        int rotationSteps = 0,
        Texture2D? pathTextureOverride = null
    )
    {
        if (!TryGetTile(tileIndex, out var tileView))
        {
            return false;
        }

        tileView.SetPath(edgeIndex, pathKind, rotationSteps, pathTextureOverride);
        return true;
    }

    public bool TryGetTile(int tileIndex, out StackView tileView)
    {
        return _tileViews.TryGetValue(tileIndex, out tileView!);
    }

    public int GetTileCount()
    {
        return _tileViews.Count;
    }

    private void BuildFixedBoard()
    {
        ClearTiles();
        foreach (var placement in _defaultPlacements)
        {
            var tileView = CreateTileInstance(placement.Index, placement.Position);
            tileView.ConfigureTileStack(
                placement.DownType,
                placement.UpType,
                placement.ConflictHighlight
            );
            _tileViews[placement.Index] = tileView;
        }
    }

    private void ClearTiles()
    {
        foreach (Node child in _cluster.GetChildren())
        {
            child.QueueFree();
        }
        _tileViews.Clear();
    }

    private StackView CreateTileInstance(int tileIndex, Vector2 position)
    {
        var scene = ResolveTileScene();
        var tileView = scene.Instantiate<StackView>();
        tileView.Name = $"Tile{tileIndex}";
        tileView.Position = position;
        _cluster.AddChild(tileView);
        return tileView;
    }

    private PackedScene ResolveTileScene()
    {
        if (TileScene != null)
        {
            return TileScene;
        }

        TileScene = GD.Load<PackedScene>("res://scenes/stacks/Stack.tscn");
        if (TileScene == null)
        {
            GD.PushError("HiveBoardView: TileScene is not assigned and fallback load failed.");
            return new PackedScene();
        }

        return TileScene;
    }

    public readonly struct TilePlacement
    {
        public TilePlacement(
            int index,
            Vector2 position,
            StackView.TileKind downType,
            StackView.TileKind? upType,
            bool conflictHighlight = false
        )
        {
            Index = index;
            Position = position;
            DownType = downType;
            UpType = upType;
            ConflictHighlight = conflictHighlight;
        }

        public int Index { get; }
        public Vector2 Position { get; }
        public StackView.TileKind DownType { get; }
        public StackView.TileKind? UpType { get; }
        public bool ConflictHighlight { get; }
    }
}
