using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Messages;

public class PlayerLeftWebSocketMessage : WebSocketMessage<UserInMatchDTO>
{
    public PlayerLeftWebSocketMessage(UserInMatchDTO user) : base(user, MessageType.PlayerLeft)
    {
    }
}
