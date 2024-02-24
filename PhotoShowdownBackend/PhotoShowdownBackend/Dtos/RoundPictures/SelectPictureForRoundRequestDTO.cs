namespace PhotoShowdownBackend.Dtos.RoundPictures;

public class SelectPictureForRoundRequestDTO
{
    public int PictureId { get; set; }
    public int MatchId { get; set; }
    public int RoundIndex { get; set; }
}
