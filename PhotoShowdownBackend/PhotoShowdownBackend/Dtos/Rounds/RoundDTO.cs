using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Dtos.Rounds
{
    public class RoundDTO
    {
        public int MatchId { get; set; }
        public int RoundIndex { get; set; } = 0;
        public Round.RoundStates RoundState { get; set; } = Round.RoundStates.NotStarted;
        public DateTime StartDate { get; set; }
        public string Sentence { get; set; } = string.Empty;
    }
}
