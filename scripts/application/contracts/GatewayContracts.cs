using System;

public interface IGameGateway
{
	event Action<string>? ServerPacketReceived;

	void Initialize();
	void Poll();
	void SendPacket(string packetJson);
	void Shutdown();
}
