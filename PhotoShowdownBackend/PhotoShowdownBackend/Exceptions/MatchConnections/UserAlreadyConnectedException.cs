namespace PhotoShowdownBackend.Exceptions.MatchConnections;

public class UserAlreadyConnectedException : AbstractException
{
    public UserAlreadyConnectedException() : base("User is already connected to a match") { }
}
