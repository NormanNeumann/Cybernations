using Godot;
using System.Collections.Generic;

public partial class MainUI
{
    private void BuildChatLogPanel(Control parent)
    {
        _chatCollapseBackdrop = new Button();
        _chatCollapseBackdrop.AnchorRight = 1.0f;
        _chatCollapseBackdrop.AnchorBottom = 1.0f;
        _chatCollapseBackdrop.GrowHorizontal = Control.GrowDirection.Both;
        _chatCollapseBackdrop.GrowVertical = Control.GrowDirection.Both;
        _chatCollapseBackdrop.Flat = true;
        _chatCollapseBackdrop.Text = string.Empty;
        _chatCollapseBackdrop.Modulate = new Color(1, 1, 1, 0);
        _chatCollapseBackdrop.FocusMode = Control.FocusModeEnum.None;
        _chatCollapseBackdrop.Visible = false;
        _chatCollapseBackdrop.ZIndex = 83;
        _chatCollapseBackdrop.Pressed += CollapseChatLog;
        parent.AddChild(_chatCollapseBackdrop);

        _chatLogPanel = CreateRoundedPanel(Shift(new Vector2(1305, 618)), new Vector2(500, 350), Color.FromHtml("#F1F1F1"), 0);
        _chatLogPanel.ClipContents = true;
        _chatLogPanel.ZIndex = 84;
        parent.AddChild(_chatLogPanel);

        _chatLogPanel.AddChild(CreateTextLabel("Chat Log", 22, _textColor, new Vector2(18, 14), new Vector2(460, 32), HorizontalAlignment.Left));

        _chatLogBodyLabel = new Label();
        _chatLogBodyLabel.Position = new Vector2(18, 52);
        _chatLogBodyLabel.Size = new Vector2(464, 280);
        _chatLogBodyLabel.HorizontalAlignment = HorizontalAlignment.Left;
        _chatLogBodyLabel.VerticalAlignment = VerticalAlignment.Top;
        _chatLogBodyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _chatLogBodyLabel.ClipText = false;
        _chatLogBodyLabel.AddThemeFontSizeOverride("font_size", 14);
        _chatLogBodyLabel.AddThemeColorOverride("font_color", _textColor);
        _chatLogPanel.AddChild(_chatLogBodyLabel);

        _chatLogHitArea = new Button();
        _chatLogHitArea.Position = _chatLogPanel.Position;
        _chatLogHitArea.Size = _chatLogPanel.Size;
        _chatLogHitArea.Flat = true;
        _chatLogHitArea.Text = string.Empty;
        _chatLogHitArea.Modulate = new Color(1, 1, 1, 0);
        _chatLogHitArea.FocusMode = Control.FocusModeEnum.None;
        _chatLogHitArea.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        _chatLogHitArea.ZIndex = 85;
        _chatLogHitArea.Pressed += ExpandChatLog;
        parent.AddChild(_chatLogHitArea);

        RefreshChatLogDisplay();
    }

    private void ExpandChatLog()
    {
        if (_isChatExpanded)
        {
            return;
        }

        _isChatExpanded = true;
        _chatCollapseBackdrop.Visible = true;
        _chatLogPanel.Position = Shift(new Vector2(1305, 34));
        _chatLogPanel.Size = new Vector2(500, 934);
        _chatLogHitArea.Position = _chatLogPanel.Position;
        _chatLogHitArea.Size = _chatLogPanel.Size;
        _chatLogBodyLabel.Size = new Vector2(464, 864);
        RefreshChatLogDisplay();
    }

    private void CollapseChatLog()
    {
        if (!_isChatExpanded)
        {
            return;
        }

        _isChatExpanded = false;
        _chatCollapseBackdrop.Visible = false;
        _chatLogPanel.Position = Shift(new Vector2(1305, 618));
        _chatLogPanel.Size = new Vector2(500, 350);
        _chatLogHitArea.Position = _chatLogPanel.Position;
        _chatLogHitArea.Size = _chatLogPanel.Size;
        _chatLogBodyLabel.Size = new Vector2(464, 280);
        RefreshChatLogDisplay();
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
        if (_chatLogBodyLabel == null)
        {
            return;
        }

        var visibleLineCount = Mathf.Max(1, GetChatVisibleLineCount());
        var visibleMessages = GetLatestMessagesForVisibleLines(visibleLineCount);
        _chatLogBodyLabel.Text = string.Join("\n", visibleMessages);
    }

    private List<string> GetLatestMessagesForVisibleLines(int lineBudget)
    {
        if (_chatMessages.Count == 0)
        {
            return new List<string>();
        }

        var font = _chatLogBodyLabel.GetThemeFont("font");
        var fontSize = _chatLogBodyLabel.GetThemeFontSize("font_size");
        if (font == null || fontSize <= 0)
        {
            var fallbackStart = Mathf.Max(0, _chatMessages.Count - lineBudget);
            return _chatMessages.GetRange(fallbackStart, _chatMessages.Count - fallbackStart);
        }

        var maxWidth = Mathf.Max(1.0f, _chatLogBodyLabel.Size.X);
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
                    // Always keep the newest message visible even if it alone exceeds the budget.
                    visible.Insert(0, message);
                }

                break;
            }

            visible.Insert(0, message);
            usedLines += neededLines;
        }

        return visible;
    }

    private int GetChatVisibleLineCount()
    {
        const int fallbackCollapsedLines = 8;
        const int fallbackExpandedLines = 22;
        const int safetyReserveLines = 1;

        var fallback = _isChatExpanded ? fallbackExpandedLines : fallbackCollapsedLines;

        var font = _chatLogBodyLabel.GetThemeFont("font");
        var fontSize = _chatLogBodyLabel.GetThemeFontSize("font_size");
        if (font == null || fontSize <= 0)
        {
            return fallback;
        }

        var lineSpacing = _chatLogBodyLabel.HasThemeConstant("line_spacing")
            ? _chatLogBodyLabel.GetThemeConstant("line_spacing")
            : 0;
        var lineHeight = font.GetHeight(fontSize) + lineSpacing;
        if (lineHeight <= 0.0f)
        {
            return fallback;
        }

        var measuredLineCount = Mathf.FloorToInt(_chatLogBodyLabel.Size.Y / lineHeight) - safetyReserveLines;
        return Mathf.Max(1, measuredLineCount);
    }

    private int EstimateWrappedLineCount(string text, Font font, int fontSize, float maxWidth)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 1;
        }

        var total = 0;
        var physicalLines = text.Split('\n');
        foreach (var physicalLine in physicalLines)
        {
            total += EstimateSinglePhysicalLineCount(physicalLine, font, fontSize, maxWidth);
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

    private float MeasureTextWidth(string text, Font font, int fontSize)
    {
        return font.GetStringSize(text, HorizontalAlignment.Left, -1, fontSize).X;
    }
}
