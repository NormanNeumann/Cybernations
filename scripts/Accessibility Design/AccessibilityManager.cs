using System;

public static class AccessibilityManager
{
	public static bool IsColorblindMode { get; private set; } = false;

	public static event Action? OnAccessibilityChanged;

	public static void ToggleColorblindMode()
	{
		IsColorblindMode = !IsColorblindMode;
		OnAccessibilityChanged?.Invoke();
	}
}
