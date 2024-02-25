namespace PhotoShowdownBackend.Exceptions.Matches;

public class UserIsNotMatchOwnerException: AbstractException
{
    public UserIsNotMatchOwnerException() : base("User is not the owner of the match")
    {
    }
}
