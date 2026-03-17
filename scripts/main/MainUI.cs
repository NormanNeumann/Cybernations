using Godot;
using System.Collections.Generic;

public partial class MainUI : Control
{
    private static readonly Vector2 LayoutShift = new Vector2(40.0f, 0.0f);

    private const int TotalTurns = 6;
    private const int TotalResourceCells = 25;
    private const int ConflictCells = 3;
    private const float HexOuterSide = 112.0f;
    private const float HexInnerSide = 108.0f;
    private const float HexOverlayOuterSide = 84.0f;
    private const float HexOverlayInnerSide = 80.0f;

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

    private Panel _teamGoalPreviewPanel = null!;
    private Button _teamGoalHitArea = null!;
    private Button _teamGoalBackdrop = null!;
    private Panel _teamGoalDropdownPanel = null!;

    private Button _chatCollapseBackdrop = null!;
    private Panel _chatLogPanel = null!;
    private Button _chatLogHitArea = null!;
    private Label _chatLogBodyLabel = null!;
    private LineEdit _chatInputLineEdit = null!;
    private bool _isChatExpanded;

    private Button _playerDetailBackdrop = null!;
    private Panel _playerDetailPanel = null!;
    private Label _playerDetailTitleLabel = null!;
    private Label _playerDetailBodyLabel = null!;

    private readonly PlayerData[] _players =
    {
        new PlayerData(1, "0.0%", true),
        new PlayerData(2, "66.7%", true),
        new PlayerData(3, "10.3%", false),
        new PlayerData(4, "90.7%", true),
        new PlayerData(5, "33.3%", false),
    };

    private readonly List<string> _chatMessages =
        new()
        {
            "[P1] Secure the wilds.",
            "[P3] Human build next turn.",
            "[P5] Conflict blocks the final cells.",
        };

    private readonly HexTileData[] _hexTiles =
    {
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
        BuildTeamGoalDropdown(canvas);
        BuildPlayerDetailPopup(canvas);
    }

    private Vector2 Shift(Vector2 position)
    {
        return position + LayoutShift;
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
