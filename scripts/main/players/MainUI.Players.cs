using Godot;

public partial class MainUI
{
    private static readonly PackedScene PlayerScene = GD.Load<PackedScene>("res://scenes/players/Player.tscn");

    private void BuildPlayerColumn(Control parent)
    {
        for (int index = 0; index < _players.Length; index++)
        {
            var player = _players[index];
            var view = PlayerScene.Instantiate<PlayerView>();
            view.Configure(player.Slot, player.IndividualProcess, player.IsPassing);
            view.PlayerSelected += () =>
            {
                var preferred = view.GlobalPosition + new Vector2(view.Size.X + 14.0f, 0.0f);
                _playerDetailPopupView.ShowPopup(player.Slot, player.IndividualProcess, preferred);
            };
            parent.AddChild(view);
        }
    }
}
