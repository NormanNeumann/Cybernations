using Godot;
using System.Collections.Generic;

public partial class ChatPanelView : Control
{
    private static readonly Vector2 LayoutShift = new Vector2(40.0f, 0.0f);
    private static readonly Vector2 RootPosition = Shift(new Vector2(1305, 34));
    private static readonly Vector2 CollapsedLogPosition = new Vector2(0.0f, 584.0f);
    private static readonly Vector2 ExpandedLogPosition = Vector2.Zero;
    private static readonly Vector2 InputPanelPosition = new Vector2(0.0f, 952.0f);
    private static readonly Vector2 InputLinePosition = new Vector2(18.0f, 965.0f);

    private readonly Color _textColor = Color.FromHtml("#16222B");

    private Panel _chatLogPanel = null!;
    private Button _chatLogHitArea = null!;
    private Panel _chatInputPanel = null!;
    private Label _chatBodyLabel = null!;
    private LineEdit _chatInputLineEdit = null!;
    private bool _isExpanded;

    private readonly List<string> _chatMessages =
    [
        "[P1] Secure the wilds.",
        "[P3] Human build next turn.",
        "[P5] Conflict blocks the final cells.",
    ];

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        SetAnchorsPreset(LayoutPreset.TopLeft);
        Position = RootPosition;
        Size = new Vector2(500, 1008);
        CustomMinimumSize = Size;

        BuildChatLogPanel();
        BuildInputPanel();
    }

    private void BuildChatLogPanel()
    {
        _chatLogPanel = CreateRoundedPanel(CollapsedLogPosition, new Vector2(500, 350), Color.FromHtml("#F1F1F1"), 0);
        _chatLogPanel.ClipContents = true;
        _chatLogPanel.ZIndex = 84;
        AddChild(_chatLogPanel);

        _chatLogPanel.AddChild(CreateTextLabel("Chat Log", 22, _textColor, new Vector2(18, 14), new Vector2(460, 32), HorizontalAlignment.Left));

        _chatBodyLabel = new Label();
        _chatBodyLabel.Position = new Vector2(18, 52);
        _chatBodyLabel.Size = new Vector2(464, 280);
        _chatBodyLabel.HorizontalAlignment = HorizontalAlignment.Left;
        _chatBodyLabel.VerticalAlignment = VerticalAlignment.Top;
        _chatBodyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _chatBodyLabel.ClipText = false;
        _chatBodyLabel.AddThemeFontSizeOverride("font_size", 14);
        _chatBodyLabel.AddThemeColorOverride("font_color", _textColor);
        _chatLogPanel.AddChild(_chatBodyLabel);

        _chatLogHitArea = new Button();
        _chatLogHitArea.Position = _chatLogPanel.Position;
        _chatLogHitArea.Size = _chatLogPanel.Size;
        _chatLogHitArea.Flat = true;
        _chatLogHitArea.Text = string.Empty;
        _chatLogHitArea.Modulate = new Color(1, 1, 1, 0);
        _chatLogHitArea.FocusMode = FocusModeEnum.None;
        _chatLogHitArea.MouseDefaultCursorShape = CursorShape.PointingHand;
        _chatLogHitArea.ZIndex = 85;
        _chatLogHitArea.Pressed += ExpandChatLog;
        AddChild(_chatLogHitArea);

        RefreshChatLogDisplay();
    }

    private void BuildInputPanel()
    {
        _chatInputPanel = CreateRoundedPanel(InputPanelPosition, new Vector2(500, 56), Color.FromHtml("#F1F1F1"), 16);
        AddChild(_chatInputPanel);

        _chatInputLineEdit = new LineEdit();
        _chatInputLineEdit.Position = InputLinePosition;
        _chatInputLineEdit.Size = new Vector2(464, 30);
        _chatInputLineEdit.PlaceholderText = "Type a message...";
        _chatInputLineEdit.TextSubmitted += OnChatInputSubmitted;
        AddChild(_chatInputLineEdit);
    }

    private void ExpandChatLog()
    {
        if (_isExpanded)
        {
            return;
        }

        _isExpanded = true;
        _chatLogPanel.Position = ExpandedLogPosition;
        _chatLogPanel.Size = new Vector2(500, 934);
        _chatLogHitArea.Position = _chatLogPanel.Position;
        _chatLogHitArea.Size = _chatLogPanel.Size;
        _chatBodyLabel.Size = new Vector2(464, 864);
        RefreshChatLogDisplay();
    }

    private void CollapseChatLog()
    {
        if (!_isExpanded)
        {
            return;
        }

        _isExpanded = false;
        _chatLogPanel.Position = CollapsedLogPosition;
        _chatLogPanel.Size = new Vector2(500, 350);
        _chatLogHitArea.Position = _chatLogPanel.Position;
        _chatLogHitArea.Size = _chatLogPanel.Size;
        _chatBodyLabel.Size = new Vector2(464, 280);
        RefreshChatLogDisplay();
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isExpanded)
        {
            return;
        }

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouseButton)
        {
            return;
        }

        var clickPoint = mouseButton.GlobalPosition;
        if (GetGlobalRect(_chatLogPanel).HasPoint(clickPoint)
            || GetGlobalRect(_chatInputPanel).HasPoint(clickPoint)
            || GetGlobalRect(_chatInputLineEdit).HasPoint(clickPoint))
        {
            return;
        }

        CollapseChatLog();
        GetViewport().SetInputAsHandled();
    }

    private void OnChatInputSubmitted(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.Length == 0)
        {
            return;
        }

        _chatMessages.Add($"[You] {trimmed}");
        _chatInputLineEdit.Clear();
        RefreshChatLogDisplay();
    }

    private void RefreshChatLogDisplay()
    {
        if (_chatBodyLabel == null)
        {
            return;
        }

        var visibleLineCount = Mathf.Max(1, GetVisibleLineCount());
        var visibleMessages = GetLatestMessagesForVisibleLines(visibleLineCount);
        _chatBodyLabel.Text = string.Join("\n", visibleMessages);
    }

    private List<string> GetLatestMessagesForVisibleLines(int lineBudget)
    {
        if (_chatMessages.Count == 0)
        {
            return [];
        }

        var font = _chatBodyLabel.GetThemeFont("font");
        var fontSize = _chatBodyLabel.GetThemeFontSize("font_size");
        if (font == null || fontSize <= 0)
        {
            var fallbackStart = Mathf.Max(0, _chatMessages.Count - lineBudget);
            return _chatMessages.GetRange(fallbackStart, _chatMessages.Count - fallbackStart);
        }

        var maxWidth = Mathf.Max(1.0f, _chatBodyLabel.Size.X);
        var usedLines = 0;
        var visible = new List<string>();

        for (int index = _chatMessages.Count - 1; index >= 0; index--)
        {
            var message = _chatMessages[index];
            var neededLines = EstimateWrappedLineCount(message, font, fontSize, maxWidth);

            if (usedLines + neededLines > lineBudget)
            {
                if (visible.Count == 0)
                {
                    visible.Insert(0, message);
                }
                break;
            }

            visible.Insert(0, message);
            usedLines += neededLines;
        }

        return visible;
    }

    private int GetVisibleLineCount()
    {
        const int fallbackCollapsed = 8;
        const int fallbackExpanded = 22;
        const int safetyReserve = 1;

        var fallback = _isExpanded ? fallbackExpanded : fallbackCollapsed;
        var font = _chatBodyLabel.GetThemeFont("font");
        var fontSize = _chatBodyLabel.GetThemeFontSize("font_size");
        if (font == null || fontSize <= 0)
        {
            return fallback;
        }

        var lineSpacing = _chatBodyLabel.HasThemeConstant("line_spacing")
            ? _chatBodyLabel.GetThemeConstant("line_spacing")
            : 0;
        var lineHeight = font.GetHeight(fontSize) + lineSpacing;
        if (lineHeight <= 0.0f)
        {
            return fallback;
        }

        var measured = Mathf.FloorToInt(_chatBodyLabel.Size.Y / lineHeight) - safetyReserve;
        return Mathf.Max(1, measured);
    }

    private int EstimateWrappedLineCount(string text, Font font, int fontSize, float maxWidth)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 1;
        }

        var total = 0;
        var physicalLines = text.Split('\n');
        foreach (var line in physicalLines)
        {
            total += EstimateSinglePhysicalLineCount(line, font, fontSize, maxWidth);
        }
        return Mathf.Max(1, total);
    }

    private int EstimateSinglePhysicalLineCount(string line, Font font, int fontSize, float maxWidth)
    {
        if (string.IsNullOrEmpty(line))
        {
            return 1;
        }

        var count = 0;
        var current = string.Empty;
        var words = line.Split(' ');
        foreach (var word in words)
        {
            var candidate = current.Length == 0 ? word : $"{current} {word}";
            if (MeasureTextWidth(candidate, font, fontSize) <= maxWidth)
            {
                current = candidate;
                continue;
            }

            if (current.Length > 0)
            {
                count++;
                current = string.Empty;
            }

            if (MeasureTextWidth(word, font, fontSize) <= maxWidth)
            {
                current = word;
                continue;
            }

            count += EstimateLongWordLineCount(word, font, fontSize, maxWidth);
        }

        if (current.Length > 0)
        {
            count++;
        }

        return Mathf.Max(1, count);
    }

    private int EstimateLongWordLineCount(string word, Font font, int fontSize, float maxWidth)
    {
        if (string.IsNullOrEmpty(word))
        {
            return 1;
        }

        var count = 0;
        var chunk = string.Empty;
        foreach (var c in word)
        {
            var candidate = chunk + c;
            if (chunk.Length == 0 || MeasureTextWidth(candidate, font, fontSize) <= maxWidth)
            {
                chunk = candidate;
                continue;
            }

            count++;
            chunk = c.ToString();
        }

        if (chunk.Length > 0)
        {
            count++;
        }
        return Mathf.Max(1, count);
    }

    private static float MeasureTextWidth(string text, Font font, int fontSize)
    {
        return font.GetStringSize(text, HorizontalAlignment.Left, -1, fontSize).X;
    }

    private static Rect2 GetGlobalRect(Control control)
    {
        return new Rect2(control.GlobalPosition, control.Size);
    }

    private static Panel CreateRoundedPanel(
        Vector2 position,
        Vector2 size,
        Color fillColor,
        int radius
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
        panel.AddThemeStyleboxOverride("panel", style);
        return panel;
    }

    private static Label CreateTextLabel(
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

    private static Vector2 Shift(Vector2 position)
    {
        return position + LayoutShift;
    }
}
