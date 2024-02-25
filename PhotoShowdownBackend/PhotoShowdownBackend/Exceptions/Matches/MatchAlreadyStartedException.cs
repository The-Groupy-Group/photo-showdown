namespace PhotoShowdownBackend.Exceptions.Matches
{
    public class MatchAlreadyStartedException : AbstractException
    {
        public MatchAlreadyStartedException() : base("This match has already started. You can no longer join it") { }
    }
}
