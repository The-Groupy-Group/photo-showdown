namespace PhotoShowdownBackend.Exceptions;

public class NotFoundException : AbstractException
{
    public NotFoundException(string? message = "resource not found") : base(message) { }
    public NotFoundException(int id) : base($"id: {id} not found") { }
}
