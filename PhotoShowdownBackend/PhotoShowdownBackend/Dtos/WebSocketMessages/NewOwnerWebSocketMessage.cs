using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Messages;

public class NewOwnerWebSocketMessage : WebSocketMessage<UserInMatchDTO>
{

    public NewOwnerWebSocketMessage(UserInMatchDTO user) : base(user, MessageType.NewOwner)
    {
    }
}
