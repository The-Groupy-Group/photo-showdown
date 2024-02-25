using PhotoShowdownBackend.Consts;

namespace PhotoShowdownBackend.Services.Session;

public interface ISessionService
{
    public string GetCurrentUserName();

    public int GetCurrentUserId();

    public IEnumerable<string> GetCurrentUserRoles();

    public bool IsCurrentUserInRole(string role);

}
