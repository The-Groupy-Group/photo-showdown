namespace PhotoShowdownBackend.Exceptions.Matches;

public class MatchAlreadyEndedException : AbstractException
{
    public MatchAlreadyEndedException() : base("Match has already ended")
    {
    }
}
