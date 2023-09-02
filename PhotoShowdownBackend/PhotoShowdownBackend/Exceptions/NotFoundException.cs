namespace PhotoShowdownBackend.Exceptions;

public class NotFoundException: AbstractException
{
    public NotFoundException(string? message = "Not Found") : base(message)
    {
        
    }
}
