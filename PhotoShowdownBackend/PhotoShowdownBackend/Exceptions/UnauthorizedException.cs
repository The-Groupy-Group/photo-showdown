namespace PhotoShowdownBackend.Exceptions;

public class UnauthorizedException : AbstractException
{
    public UnauthorizedException(string? message = "Not Found") : base(message) { }
}
