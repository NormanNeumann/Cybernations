using System;
using Godot;

public class MockEnvisionGateway : IEnvisionGateway
{
	public event Action<EnvisionUiState>? OnStateUpdated;

	public void SendAction(EnvisionActionRequest request)
	{
		GD.Print(

			$"Presenter received envision request: {request.Action}, " +
			$"Mode={request.Mode}, " +
			$"SelectedFeedbackTrackIndex={request.SelectedFeedbackTrackIndex}, " +
			$"TrackTokenType={request.TrackTokenType}, " +
			$"DrawnTokenType1={request.DrawnTokenType1}, " +
			$"DrawnTokenType2={request.DrawnTokenType2}, " +
			$"TokenToTrack={request.TokenToTrack}, " +
			$"TokenToBag={request.TokenToBag}, " +
			$"TokenToReserve={request.TokenToReserve}"
		);
		
		var newState = new EnvisionUiState
		{
			IsVisible = false,
			CurrentPlayerId = 0,
			LocalPlayerId = 0,
			IsLocalPlayersTurn = false,

			Players = new[]
			{
				new PlayerState
				{
					Id = 0,
					People = 2,
					Environment = 2,
					Technology = 2,
					Cybernation = 0,
					Cohesion = 5
				}
			},

			CanShiftPower = true,
			CanComeTogether = true,
			CanConnect = true,
			CanSetCourse = true,
			CanPrepare = true,
			CanSteer = true,
			CanPass = true,

			StatusMessage = BuildStatusMessage(request)
		};

		OnStateUpdated?.Invoke(newState);
	}

	private static string BuildStatusMessage(EnvisionActionRequest request)
	{
		if (request.Action == "Steer" && request.Mode == "ManipulateTokens")
{
	return $"[MOCK] Steer completed: Track={request.TokenToTrack}, Bag={request.TokenToBag}, Reserve={request.TokenToReserve}";
}
		if (request.Action == "Steer" && request.Mode == "AddReserveToken")
		{
			return $"[MOCK] Steer added {request.FeedbackTokenType} Feedback to the Bag";
		}

		if (request.Action == "Connect")
		{
			return $"[MOCK] Connect: spent {request.SpendType}, gained {request.GainType}";
		}

		if (request.Action == "ShiftPower")
		{
			return $"[MOCK] Shift Power: First Player token moved to Player {request.TargetPlayerId + 1}";
		}

		if (request.Action == "SetCourse")
		{
			return $"[MOCK] Set Course: {request.Mode}";
		}

		return $"[MOCK] {request.Action} executed";
	}
}
