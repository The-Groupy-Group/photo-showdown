using PhotoShowdownBackend.Repositories.Users;

namespace PhotoShowdownBackend.Services.Users;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public async Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO registerationRequest)
    {
        throw new NotImplementedException();
    }
}
