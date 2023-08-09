namespace PhotoShowdownBackend.Exceptions.Users;

public class UsersServiceException : Exception
{
    public UsersServiceException() { }

    public UsersServiceException(string message) : base(message) { }

    public UsersServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}
