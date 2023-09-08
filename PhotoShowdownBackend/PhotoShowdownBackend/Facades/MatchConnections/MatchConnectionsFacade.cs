using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Facades.Matches;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Facades.MatchConnections;

public class MatchConnectionsFacade : IMatchConnectionsFacade
{
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly IUsersService _usersService;
    private readonly IMatchesService _matchesService;
    private readonly ILogger<MatchConnectionsFacade> _logger;

    public MatchConnectionsFacade(IMatchConnectionsService matchConnectionsService, IUsersService usersService, IMatchesService matchesService, ILogger<MatchConnectionsFacade> logger)
    {
        _matchConnectionsService = matchConnectionsService;
        _usersService = usersService;
        _matchesService = matchesService;
        _logger = logger;
    }

    public async Task CreateMatchConnection(int userId, int matchId)
    {
        if (!await _usersService.DoesUserExist(userId))
        {
            throw new NotFoundException("Invalid user Id");
        }

        if (!await _matchesService.DoesMatchExists(matchId))
        {
            throw new NotFoundException("Invalid match Id");
        }

        await _matchConnectionsService.CreateMatchConnection(userId, matchId);
    }

    public async Task<bool> UserConnectedToMatch(int userId)
    {
        return await _matchConnectionsService.IsUserConnectedToMatch(userId);
    }

    public async Task LeaveMatch(int userId, int matchId)
    {

        await _matchConnectionsService.CloseConnection(userId,matchId);

        if (await _matchConnectionsService.IsMatchEmpty(matchId))
        {
            await _matchesService.CloseMatch(matchId);
        }

    }
}
