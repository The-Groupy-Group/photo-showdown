using PhotoShowdownBackend.Dtos.Pictures;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Dtos.Rounds;

public class RoundDTO
{
    public int MatchId { get; set; }
    public int RoundIndex { get; set; } = 0;
    public Round.RoundStates RoundState { get; set; }
    public DateTime StartDate { get; set; }
    public string Sentence { get; set; } = string.Empty;
    public List<PictureSelectedDTO> PicturesSelected { get; set; } = new();
    public UserPublicDetailsDTO? RoundWinner { get; set; }
    public DateTime PictureSelectionEndDate { get; set; }
    public DateTime VotingEndDate { get; set; }
    public DateTime RoundEndDate { get; set; }
}
