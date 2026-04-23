using System;

public class MockEnvisionGateway : IEnvisionGateway
{
	public event Action<EnvisionUiState>? OnStateUpdated;

	public void SendAction(EnvisionActionRequest request)
	{
		var newState = new EnvisionUiState
		{
			IsVisible = false,
			CurrentPlayerId = 0,
			LocalPlayerId = 0,
			IsLocalPlayersTurn = false,

			Players = new[]
			{
				new PlayerState { Id = 0, People = 2, Environment = 2, Technology = 2, Cybernation = 0, Cohesion = 5 }
			},

			CanShiftPower = true,
			CanComeTogether = true,
			CanConnect = true,
			CanSetCourse = true,
			CanPrepare = true,
			CanSteer = true,
			CanPass = true,

			StatusMessage = $"[MOCK] {request.Action} executed"
		};

		OnStateUpdated?.Invoke(newState);
	}
}
