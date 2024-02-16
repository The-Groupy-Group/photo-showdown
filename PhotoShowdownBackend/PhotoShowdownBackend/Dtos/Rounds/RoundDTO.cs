namespace PhotoShowdownBackend.Dtos.Rounds
{
    public class RoundDTO
    {
        public int MatchId { get; set; } = 0;
        public int RoundIndex { get; set; } = 1;
        public DateTime StartDate { get; set; }
        public string Sentence { get; set; } = string.Empty;


    }
}
