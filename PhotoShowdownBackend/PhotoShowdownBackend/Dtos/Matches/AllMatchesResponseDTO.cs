using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Dtos.Matches;

public class AllMatchesResponseDTO
{
    public List<Match> Matches {  get; set; } = new List<Match>();
}
