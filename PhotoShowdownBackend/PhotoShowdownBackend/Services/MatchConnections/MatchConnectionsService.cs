using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Services.MatchConnections;

public class MatchConnectionsService : IMatchConnectionsService
{
    private readonly IMatchConnectionsRepository _matchConnectionsRepo;
    private readonly IUsersService _usersService;
    private readonly IMatchesService _matchesService;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchConnectionsService> _logger;

    public MatchConnectionsService(IMatchConnectionsRepository matchConnectionsRepo,IUsersService usersService,IMatchesService matchesService, IMapper mapper, ILogger<MatchConnectionsService> logger)
    {
        _matchConnectionsRepo = matchConnectionsRepo;
        _usersService = usersService;
        _matchesService = matchesService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateMatchConnection(int userId, int matchId)
    {

        if(await _usersService.UserExists(userId))
        {
            throw new NotFoundException("Invalid user Id");
        }

        if(await _matchesService.MatchExists(matchId))
        {
            throw new NotFoundException("Invalid match Id");
        }

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
