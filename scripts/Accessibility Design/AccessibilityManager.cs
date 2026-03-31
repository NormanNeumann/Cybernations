using System;

public static class AccessibilityManager
{
	public static AccessibilityMode CurrentMode { get; private set; } = AccessibilityMode.Off;

	public static event Action? OnAccessibilityChanged;

	public static void CycleMode()
	{
		CurrentMode = CurrentMode switch
		{
			AccessibilityMode.Off => AccessibilityMode.GlobalFilter,
			AccessibilityMode.GlobalFilter => AccessibilityMode.BoardRecolor,
			_ => AccessibilityMode.Off
		};

		OnAccessibilityChanged?.Invoke();
	}

	public static bool IsGlobalFilterEnabled =>
		CurrentMode == AccessibilityMode.GlobalFilter;

	public static bool IsBoardRecolorEnabled =>
		CurrentMode == AccessibilityMode.BoardRecolor;
}
