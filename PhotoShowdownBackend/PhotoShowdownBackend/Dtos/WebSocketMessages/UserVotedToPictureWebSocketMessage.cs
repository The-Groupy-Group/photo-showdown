namespace PhotoShowdownBackend.Dtos.WebSocketMessages
{
    public class UserVotedToPictureWebSocketMessage : WebSocketMessage
    {
        public UserVotedToPictureWebSocketMessage() : base(MessageType.UserVotedToPicture)
        {

        }
    }
}
