using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Facades.Matches;

public class MatchesFacade : IMatchesFacade
{
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly IMatchesService _matchesService;
    private readonly ILogger<MatchesFacade> _logger;

    public MatchesFacade(IMatchConnectionsService matchConnectionsService, IMatchesService matchesService, ILogger<MatchesFacade> logger)
    {
        _matchConnectionsService = matchConnectionsService;
        _matchesService = matchesService;
        logger = logger;
    }


    public async Task<MatchCreationResponseDTO> CreateNewMatch(int userId)
    {
        if (await _matchConnectionsService.IsUserConnectedToMatch(userId))
        {
            throw new UserAlreadyConnectedException();
        }

        var newMatch = await _matchesService.CreateNewMatch(userId);
        await _matchConnectionsService.CreateMatchConnection(userId, newMatch.Id);
        return newMatch;
    }

    public async Task<List<MatchDTO>> GetAllOpenMatches()
    {
        return await _matchesService.GetAllOpenMatches();

    }

    public async Task<bool> MatchExists(int matchId)
    {
        return await _matchesService.DoesMatchExists(matchId);
    }

    public async Task<MatchDTO> GetMatchById(int matchId)
    {
        return await _matchesService.GetMatchById(matchId);
    }
}
