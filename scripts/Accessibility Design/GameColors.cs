using Godot;

public static class GameColors
{
	public static Color Wilds =>
		AccessibilityManager.IsColorblindMode ? new Color("#009E73") : new Color("#4CAF50");

	public static Color Wastes =>
		AccessibilityManager.IsColorblindMode ? new Color("#D55E00") : new Color("#F28C38");

	public static Color Works =>
		AccessibilityManager.IsColorblindMode ? new Color("#0072B2") : new Color("#29B6F6");

	public static Color Agora =>
		AccessibilityManager.IsColorblindMode ? new Color("#CC79A7") : new Color("#D870D6");

	public static Color Develop =>
		AccessibilityManager.IsColorblindMode ? new Color("#F0E442") : new Color("#F5E400");

	public static Color Transform =>
		AccessibilityManager.IsColorblindMode ? new Color("#000000") : new Color("#FF1E1E");
}
