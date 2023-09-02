using AutoMapper;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Repositories.Users;

namespace PhotoShowdownBackend.Services.MatchConnections;

public class MatchConnectionsService : IMatchConnectionsService
{
    private readonly IMatchConnectionsRepository _matchConnectionsRepo;
    private readonly IUsersRepository _usersRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchConnectionsService> _logger;

    public MatchConnectionsService(IMatchConnectionsRepository matchConnectionsRepo, IUsersRepository usersRepository, IMapper mapper, ILogger<MatchConnectionsService> logger)
    {
        _matchConnectionsRepo = matchConnectionsRepo;
        _usersRepo = usersRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateMatchConnection(int userId, int matchId)
    {
        if (await _usersRepo.IsConnected(userId))
        {
            throw new UserAlreadyConnectedException();
        }

        var matchConnection = new MatchConnection()
        {
            UserId = userId,
            MatchId = matchId
        };

        await _matchConnectionsRepo.CreateAsync(matchConnection);
    }

}
