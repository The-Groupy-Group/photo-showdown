namespace PhotoShowdownBackend.Exceptions;

public class NotFoundException: AbstractException
{
    public NotFoundException(): base("Not Found")
    {
        
    }
}
