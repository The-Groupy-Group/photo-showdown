namespace PhotoShowdownBackend.Repositories.Users;

public class UsersRepository: Repository<User>, IUsersRepository
{
    public UsersRepository(PhotoShowdownDbContext _db) : base(_db)
    {
    }

    public async Task<bool> IsUniqueUser(string username, string email)
    {
        return await _dbSet.AllAsync(u => u.UserName != username && u.Email != email);
    }
}

