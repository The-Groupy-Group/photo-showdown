using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.WebSockets.Messages;

public class PlayerLeftWebSocketMessage : WebSocketMessage
{
    public UserPublicDetailsDTO User { get; set; }
    public PlayerLeftWebSocketMessage(UserPublicDetailsDTO user) : base(MessageType.PlayerLeft)
    {
        User = user;
    }
}
