using AutoMapper;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Services.MatchConnections;

public class MatchConnectionsService : IMatchConnectionsService
{
    private readonly IMatchConnectionsRepository _matchConnectionsRepo;
    private readonly IUsersRepository _usersRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchConnectionsService> _logger;

    public MatchConnectionsService(IMatchConnectionsRepository matchConnectionsRepo, IUsersRepository usersRepository, IConfiguration configuration, IMapper mapper, ILogger<MatchConnectionsService> logger)
    {
        _matchConnectionsRepo = matchConnectionsRepo;
        _usersRepository = usersRepository;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateMatchConnection(int userId, int matchId)
    {
        var matchConnection = new MatchConnection()
        {
            UserId = userId,
            MatchId = matchId
        };

        if (await _usersRepository.IsConnected(userId))
        {
            throw new UserAlreadyConnectedException();
        }

        var createdMatchConnection = await _matchConnectionsRepo.CreateAsync(matchConnection);


    }

}
