using System.ComponentModel;

namespace PhotoShowdownBackend.Dtos.WebSocketMessages;

public class MatchStartedWebSocketMessage : WebSocketMessage
{
    public MatchStartedWebSocketMessage() : base(MessageType.MatchStarted)
    {
    }
}
