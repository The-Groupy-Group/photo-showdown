using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.WebSockets.Messages;

public class NewOwnerWebSocketMessage : WebSocketMessage<UserPublicDetailsDTO>
{

    public NewOwnerWebSocketMessage(UserPublicDetailsDTO user) : base(user, MessageType.NewOwner)
    {
    }
}
