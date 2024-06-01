using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Messages;

public class PlayerJoinedWebSocketMessage : WebSocketMessage<UserInMatchDTO>
{
    public PlayerJoinedWebSocketMessage(UserInMatchDTO user) : base(user, MessageType.PlayerJoined)
    {
    }
}
