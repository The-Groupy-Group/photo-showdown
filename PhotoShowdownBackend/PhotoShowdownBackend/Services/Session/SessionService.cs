using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Models;
using System.Security.Claims;

namespace PhotoShowdownBackend.Services.Session;

public class SessionService : ISessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Ctor
    public SessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string GetCurrentUserName()
    {
        // Get current user name
        string? currnetUsername = _httpContextAccessor.HttpContext!.User.FindFirstValue(UserClaims.Username);
        
        if (string.IsNullOrEmpty(currnetUsername))
        {
            throw new Exception("No user name found for current session");
        }
        return currnetUsername;
    }
    public int GetCurrentUserId()
    {
        int? id = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(UserClaims.Id)!);
        if (!id.HasValue)
        {
            throw new Exception("No id found for current session");
        }
        return id.Value;
    }
    public IEnumerable<string> GetCurrentUserRoles()
    {
        return _httpContextAccessor.HttpContext!.User.Claims.Where(c => c.Type == UserClaims.Roles).Select(c => c.Value);
    }
    public bool IsCurrentUserInRole(string role)
    {
        return _httpContextAccessor.HttpContext!.User.IsInRole(role);
    }
}
