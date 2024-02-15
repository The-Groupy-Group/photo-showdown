using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.WebSockets.Messages;

public class PlayerJoinedWebSocketMessage : WebSocketMessage
{
    public UserPublicDetailsDTO User { get; set; }
    public PlayerJoinedWebSocketMessage(UserPublicDetailsDTO userName) : base(MessageType.PlayerJoined)
    {
        User = userName;
    }
}
