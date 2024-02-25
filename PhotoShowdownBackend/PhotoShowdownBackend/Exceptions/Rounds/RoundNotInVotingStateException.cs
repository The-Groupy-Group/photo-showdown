namespace PhotoShowdownBackend.Exceptions.Rounds;

public class RoundNotInVotingStateException: AbstractException
{
    public RoundNotInVotingStateException() : base("Round is not in voting state") { }
}
