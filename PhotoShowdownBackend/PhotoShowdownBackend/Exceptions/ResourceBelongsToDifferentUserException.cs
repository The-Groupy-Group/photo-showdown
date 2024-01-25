namespace PhotoShowdownBackend.Exceptions;

public class ResourceBelongsToDifferentUserException : AbstractException
{
    public ResourceBelongsToDifferentUserException(string? message = "This resource does not belong to the current user") : base(message) { }
}
