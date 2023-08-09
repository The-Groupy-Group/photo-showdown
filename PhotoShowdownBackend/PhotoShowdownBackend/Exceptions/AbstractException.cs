namespace PhotoShowdownBackend.Exceptions;

public class AbstractException : Exception
{
    public AbstractException(string? message = null) : base(message) { }
}
