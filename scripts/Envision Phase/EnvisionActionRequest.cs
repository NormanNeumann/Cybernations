public sealed class EnvisionActionRequest
{
	public string Action { get; set; } = "";
	public int? TargetPlayerId { get; set; }
	public string? SpendType { get; set; }
	public string? GainType { get; set; }
	public string? Mode { get; set; }
	public string? FeedbackTokenType { get; set; }
	public int? SelectedFeedbackTrackIndex { get; set; }
	public string? TrackTokenType { get; set; }
	public string? DrawnTokenType1 { get; set; }
	public string? DrawnTokenType2 { get; set; }

	public string? TokenToTrack { get; set; }
	public string? TokenToBag { get; set; }
	public string? TokenToReserve { get; set; }
}
