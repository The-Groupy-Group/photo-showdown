using PhotoShowdownBackend.Dtos.Rounds;

namespace PhotoShowdownBackend.Dtos.WebSocketMessages;

public class RoundStateChangeWebSocketMessage : WebSocketMessage<RoundDTO>
{
    public RoundStateChangeWebSocketMessage(RoundDTO data) : base(data, MessageType.RoundStateChange)
    {
    }
}
