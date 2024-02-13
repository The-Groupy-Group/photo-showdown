namespace PhotoShowdownBackend.WebSockets.Messages;

public class PlayerJoinedWebSocketMessage : WebSocketMessage
{
    public string UserName { get; set; }
    public PlayerJoinedWebSocketMessage(string userName) : base(MessageType.PlayerJoined)
    {
        UserName = userName;
    }
}
