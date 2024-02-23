namespace PhotoShowdownBackend.Exceptions.Matches
{
    public class MatchDidNotStartYetException : AbstractException
    {
        public MatchDidNotStartYetException() : base("Match did not start yet.") { }
    }
}
