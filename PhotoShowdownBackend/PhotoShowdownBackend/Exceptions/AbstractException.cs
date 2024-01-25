namespace PhotoShowdownBackend.Exceptions;

public abstract class AbstractException : Exception
{
    public AbstractException(string? message = null) : base(message) { }
}
