using Godot;
using System;

public enum EnvisionAction
{
	ShiftPower,
	ComeTogether,
	Connect,
	SetCourse,
	Prepare,
	Steer,
	Pass
}

public static class ActionCostChecker
{
	public static bool CanExecute(PlayerState p, EnvisionAction action)
	{
		switch (action)
		{
			case EnvisionAction.ShiftPower:
				return p.People >= 1;

			case EnvisionAction.ComeTogether:
				return p.Environment >= 1;

			case EnvisionAction.Connect:
				return p.Environment >= 2 || p.People >= 2 || p.Technology >= 2;

			case EnvisionAction.SetCourse:
				return p.Technology >= 2;

			case EnvisionAction.Prepare:
				return p.People >= 2;

			case EnvisionAction.Steer:
				return p.Environment >= 2;

			case EnvisionAction.Pass:
				return true;
		}

		return false;
	}
}
