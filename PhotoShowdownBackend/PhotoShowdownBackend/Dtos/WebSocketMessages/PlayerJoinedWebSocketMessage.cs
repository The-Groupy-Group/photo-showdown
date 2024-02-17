using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Messages;

public class PlayerJoinedWebSocketMessage : WebSocketMessage<UserPublicDetailsDTO>
{
    public PlayerJoinedWebSocketMessage(UserPublicDetailsDTO user) : base(user, MessageType.PlayerJoined)
    {
    }
}
