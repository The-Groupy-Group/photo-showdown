using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.WebSockets.Messages;

public class PlayerLeftWebSocketMessage : WebSocketMessage<UserPublicDetailsDTO>
{
    public PlayerLeftWebSocketMessage(UserPublicDetailsDTO user) : base(user, MessageType.PlayerLeft)
    {
    }
}
