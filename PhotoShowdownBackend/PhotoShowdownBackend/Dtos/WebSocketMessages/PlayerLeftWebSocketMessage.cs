using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Messages;

public class PlayerLeftWebSocketMessage : WebSocketMessage<UserPublicDetailsDTO>
{
    public PlayerLeftWebSocketMessage(UserPublicDetailsDTO user) : base(user, MessageType.PlayerLeft)
    {
    }
}
