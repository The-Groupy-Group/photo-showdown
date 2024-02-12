namespace PhotoShowdownBackend.Dtos.Matches;

public class CurrentMatchDTO
{
    public int Id { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public List<string> UserNames { get; set; } = new List<string>();
    public bool HasStarted { get; set; } = false;
}
