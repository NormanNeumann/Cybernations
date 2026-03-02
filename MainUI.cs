using Godot;
using System;
using System.Collections.Generic;

public partial class MainUI : Control
{
    // 颜色常量
    private readonly Color ColorWilds = Color.FromHtml("#00FF73");
    private readonly Color ColorWasted = Color.FromHtml("#D87300");
    private readonly Color ColorHuman = Color.FromHtml("#E200C4");
    private readonly Color ColorTech = Color.FromHtml("#3628FF");

    public override void _Ready()
    {
        SetupPlayers();
        SetupHexGrid();
        SetupResourceTracks();
        SetupRightPanels();
    }

    // 1. 玩家列表逻辑
    private void SetupPlayers()
    {
        var sidebar = GetNode<VBoxContainer>("LeftSidebar");
        for (int i = 1; i <= 5; i++)
        {
            var playerBox = new VBoxContainer();
            
            // 头像逻辑：如果没有设置（这里模拟null），显示 P1, P2...
            var avatar = new ColorRect { CustomMinimumSize = new Vector2(60, 60), Color = Colors.Gray };
            var label = new Label { Text = $"P{i}", HorizontalAlignment = HorizontalAlignment.Center };
            avatar.AddChild(label);
            
            // 进度条
            var progress = new ProgressBar { CustomMinimumSize = new Vector2(80, 10), Value = 30 + (i * 10) };
            
            playerBox.AddChild(avatar);
            playerBox.AddChild(progress);
            sidebar.AddChild(playerBox);
        }
    }

    // 2. 六边形生成 (简化为正方形示意，实际可用Polygon2D绘制)
    private void SetupHexGrid()
    {
        var container = GetNode<Control>("HexGridContainer");
        // 示例：生成一个带叠加态的六边形
        CreateHex(container, new Vector2(0, 0), ColorWilds, ColorHuman);
        CreateHex(container, new Vector2(120, 0), ColorWasted, Color.FromHtml("#00000000")); // 无叠加
    }

    private void CreateHex(Control parent, Vector2 pos, Color baseColor, Color overlayColor)
    {
        // 基础层
        var baseHex = new ColorRect { Size = new Vector2(100, 100), Position = pos, Color = baseColor };
        // 叠加层 (小一圈)
        if (overlayColor.A > 0) {
            var overlay = new ColorRect { Size = new Vector2(60, 60), Position = new Vector2(20, 20), Color = overlayColor };
            baseHex.AddChild(overlay);
        }
        parent.AddChild(baseHex);
    }

    // 3. 资源轨道与冲突 (Conflict) 逻辑
    private void SetupResourceTracks()
    {
        var container = GetNode<VBoxContainer>("ResourceTracks");
        string[] types = { "Human", "Tech", "Env" };
        int conflictValue = 3; // 假设当前冲突值为3

        foreach (var type in types)
        {
            var track = new HBoxContainer();
            for (int i = 0; i < 20; i++)
            {
                var cell = new ColorRect { CustomMinimumSize = new Vector2(15, 25) };
                // 核心逻辑：从右向左填充 Conflict
                if (i >= (20 - conflictValue)) 
                    cell.Color = Colors.Red; // Conflict 状态
                else if (i < 10) 
                    cell.Color = Colors.White; // Filled 状态
                else 
                    cell.Color = Colors.DimGray; // Empty 状态
                
                track.AddChild(cell);
            }
            container.AddChild(track);
        }
    }

    private void SetupRightPanels()
    {
        var right = GetNode<VBoxContainer>("RightPanel");
        string[] names = { "Major Goal", "Info Panel", "Chat Log" };
        foreach (var name in names)
        {
            var panel = new PanelContainer { CustomMinimumSize = new Vector2(0, 200) };
            panel.AddChild(new Label { Text = name });
            right.AddChild(panel);
        }
        right.AddChild(new LineEdit { PlaceholderText = "Enter message..." });
    }
}