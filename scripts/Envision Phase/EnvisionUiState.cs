using System;

public sealed class EnvisionUiState
{
	public bool IsVisible { get; set; }
	public bool IsLocalPlayersTurn { get; set; }
	public int CurrentPlayerId { get; set; }
	public int LocalPlayerId { get; set; }

	public PlayerState[] Players { get; set; } = Array.Empty<PlayerState>();

	public bool CanShiftPower { get; set; }
	public bool CanComeTogether { get; set; }
	public bool CanConnect { get; set; }
	public bool CanSetCourse { get; set; }
	public bool CanPrepare { get; set; }
	public bool CanSteer { get; set; }
	public bool CanPass { get; set; }

	public string StatusMessage { get; set; } = "";
}
