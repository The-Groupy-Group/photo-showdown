using AutoMapper;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Repositories.Users;

namespace PhotoShowdownBackend.Services.MatchConnections;

public class MatchConnectionsService : IMatchConnectionsService
{
    private readonly IMatchConnectionsRepository _matchConnectionsRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchConnectionsService> _logger;

    public MatchConnectionsService(IMatchConnectionsRepository matchConnectionsRepo, IMapper mapper, ILogger<MatchConnectionsService> logger)
    {
        _matchConnectionsRepo = matchConnectionsRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateMatchConnection(int userId, int matchId)
    {
        if (await _matchConnectionsRepo.UserConnectedToMatch(userId))
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

    public async Task<bool> UserConnectedToMatch(int userId)
    {
        return await _matchConnectionsRepo.UserConnectedToMatch(userId);
    }
}
