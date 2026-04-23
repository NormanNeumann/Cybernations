public interface IEnvisionGateway
{
	void SendAction(EnvisionActionRequest request);

	event Action<EnvisionUiState> OnStateUpdated;
}
