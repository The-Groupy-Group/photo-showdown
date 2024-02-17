using PhotoShowdownBackend.Dtos.Rounds;

namespace PhotoShowdownBackend.Dtos.WebSocketMessages;

public class NewRoundStartedWebSocketMessage : WebSocketMessage<RoundDTO>
{
    public NewRoundStartedWebSocketMessage(RoundDTO data) : base(data, MessageType.NewRoundStarted)
    {
    }
}
