namespace PhotoShowdownBackend.Exceptions.Rounds;

public class RoundNotInSelectionStateException: AbstractException
{
    public RoundNotInSelectionStateException() : base("Round is not in the picture selection state")
    {
    }
}
