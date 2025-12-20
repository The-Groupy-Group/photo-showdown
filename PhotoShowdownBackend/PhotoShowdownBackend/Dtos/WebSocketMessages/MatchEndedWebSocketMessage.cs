using System.ComponentModel;

namespace PhotoShowdownBackend.Dtos.WebSocketMessages;

public class MatchEndedWebSocketMessage : WebSocketMessage
{
    public MatchEndedWebSocketMessage() : base(MessageType.MatchEnded)
    {
    }
}
