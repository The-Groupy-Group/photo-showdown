namespace PhotoShowdownBackend.Exceptions.Rounds;

public class CantFetchSentenceException: AbstractException
{
    public CantFetchSentenceException() : base("Can't fetch sentence")
    {
    }
}
