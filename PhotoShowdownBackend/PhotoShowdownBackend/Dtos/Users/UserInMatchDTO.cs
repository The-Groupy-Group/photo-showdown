namespace PhotoShowdownBackend.Dtos.Users;

public class UserInMatchDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Score { get; set; } = 0;
    public bool isLockedIn { get; set; } = false;
}
