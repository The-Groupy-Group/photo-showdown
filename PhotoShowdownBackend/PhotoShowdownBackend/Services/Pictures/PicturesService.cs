using AutoMapper;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Services.Pictures;

public class PicturesService: IPicturesService
{
    private readonly IPicturesRepository _picturesRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersService> _logger;

    private const int TOKEN_EXPIRATION_HOURS = 5;

    public PicturesService(IUsersRepository picturesRepo, IConfiguration configuration, IMapper mapper, ILogger<UsersService> logger)
    {
        _picturesRepo = _picturesRepo;
        _mapper = mapper;
        _logger = logger;
    }
}
