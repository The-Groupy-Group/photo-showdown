namespace PhotoShowdownBackend.WebSockets.Messages;

public class PlayerLeftWebSocketMessage : WebSocketMessage
{
    public string UserName { get; set; }
    public PlayerLeftWebSocketMessage(string userName) : base(MessageType.PlayerLeft)
    {
        UserName = userName;
    }
}
