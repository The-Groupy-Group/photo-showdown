using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) interface for our Users
/// </summary>
public interface IUsersRepository: IRepository<User>
{
    public Task<bool> IsUniqueUser(string username, string email);

    public Task<bool> IsConnected(int userId);
}
