using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.WebSockets.Messages;

public class PlayerJoinedWebSocketMessage : WebSocketMessage<UserPublicDetailsDTO>
{
    public PlayerJoinedWebSocketMessage(UserPublicDetailsDTO user) : base(user, MessageType.PlayerJoined)
    {
    }
}
