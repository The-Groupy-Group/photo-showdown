using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.WebSockets.Messages;

public class NewOwnerWebSocketMessage : WebSocketMessage
{
    public UserPublicDetailsDTO User { get; set; }
    public NewOwnerWebSocketMessage(UserPublicDetailsDTO user) : base(MessageType.NewOwner)
    {
        User = user;
    }
}
