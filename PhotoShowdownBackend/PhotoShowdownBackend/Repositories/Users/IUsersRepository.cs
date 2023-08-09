using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Users;

public interface IUsersRepository: IRepository<User>
{
    public Task<bool> IsUniqueUser(string username, string email);
}
