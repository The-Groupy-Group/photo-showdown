using PhotoShowdownBackend.Dtos.RoundPictures;

namespace PhotoShowdownBackend.Dtos.WebSocketMessages;

public class UserVotedToPictureWebSocketMessage : WebSocketMessage<PictureSelectedDTO>
{

    public UserVotedToPictureWebSocketMessage(PictureSelectedDTO pictureSelected) : base(pictureSelected,MessageType.UserVotedToPicture)
    {

    }
}
