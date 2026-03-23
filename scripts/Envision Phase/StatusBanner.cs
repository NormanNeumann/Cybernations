using Godot;

public partial class StatusBanner : Control
{
	private Label _label = null!;
	private int _messageVersion = 0;

	public override void _Ready()
	{
		_label = GetNode<Label>("BannerPanel/MarginContainer/Label");
		_label.Text = "";
	}

	public void ShowMessage(string msg, Color? color = null)
	{
		_messageVersion++;
		_label.Text = msg;
		_label.Modulate = color ?? Colors.White;
	}

	public async void ShowTemporaryMessage(string msg, float seconds = 2.0f, Color? color = null)
	{
		_messageVersion++;
		int currentVersion = _messageVersion;

		_label.Text = msg;
		_label.Modulate = color ?? Colors.White;

		await ToSignal(GetTree().CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);

		if (currentVersion == _messageVersion)
		{
			_label.Text = "";
		}
	}

	public void ClearMessage()
	{
		_messageVersion++;
		_label.Text = "";
	}
}
