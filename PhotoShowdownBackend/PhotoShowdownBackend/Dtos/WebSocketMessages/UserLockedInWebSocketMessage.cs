using PhotoShowdownBackend.Dtos.RoundPictures;

namespace PhotoShowdownBackend.Dtos.WebSocketMessages;

public class UserLockedInWebSocketMessage : WebSocketMessage<int>
{
    public UserLockedInWebSocketMessage(int userId) : base(userId, MessageType.UserLockedIn)
    {
    }
}
