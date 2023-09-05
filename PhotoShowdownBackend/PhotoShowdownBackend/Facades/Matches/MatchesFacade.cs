using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Matches;

namespace PhotoShowdownBackend.Facades.Matches
{
    public class MatchesFacade : IMatchesFacade
    {
        private readonly IMatchConnectionsService _matchConnectionsService;
        private readonly IMatchesService _matchesService;

        public MatchesFacade(IMatchConnectionsService matchConnectionsService, IMatchesService matchesService)
        {
            _matchConnectionsService = matchConnectionsService;
            _matchesService = matchesService;
        }


        public async Task<MatchCreationResponseDTO> CreateNewMatch(int userId)
        {
            if (await _matchConnectionsService.UserConnectedToMatch(userId))
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
            return await _matchesService.MatchExists(matchId);
        }



    }
}
