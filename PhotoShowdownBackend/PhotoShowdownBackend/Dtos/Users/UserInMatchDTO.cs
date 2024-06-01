namespace PhotoShowdownBackend.Dtos.Users;

public class UserInMatchDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsLockedIn { get; set; } = false;
    public double Score { get; set; } = 0d;
}
