namespace PhotoShowdownBackend.Exceptions.MatchConnections
{
    public class UserNotConnectedToMatchException : AbstractException
    {
        public UserNotConnectedToMatchException() : base("User not connected to this match") { }
    }
}
