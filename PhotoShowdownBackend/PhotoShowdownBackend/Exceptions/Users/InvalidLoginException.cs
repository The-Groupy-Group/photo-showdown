namespace PhotoShowdownBackend.Exceptions.Users
{
    public class InvalidLoginException : AbstractException
    {
        public InvalidLoginException() : base("Invalid Username or Password")
        {

        }
    }
}
