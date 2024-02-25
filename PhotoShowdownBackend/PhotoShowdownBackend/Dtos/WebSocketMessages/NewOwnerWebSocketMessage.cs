using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Messages;

public class NewOwnerWebSocketMessage : WebSocketMessage<UserPublicDetailsDTO>
{

    public NewOwnerWebSocketMessage(UserPublicDetailsDTO user) : base(user, MessageType.NewOwner)
    {
    }
}
