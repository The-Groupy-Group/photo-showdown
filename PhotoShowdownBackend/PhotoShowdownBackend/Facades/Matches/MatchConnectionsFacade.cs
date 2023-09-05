using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Facades.Matches
{
    public class MatchConnectionsFacade
    {
        private readonly IMatchConnectionsService _matchConnectionsService;
        private readonly IUsersService _usersService;
        private readonly IMatchesService _matchesService;

        public MatchConnectionsFacade(IMatchConnectionsService matchConnectionsService,IUsersService usersService,IMatchesService matchesService)
        {
            _matchConnectionsService = matchConnectionsService;
            _usersService = usersService;
            _matchesService = matchesService;
        }

        public async Task CreateMatchConnection(int userId, int matchId)
        {
            if (await _usersService.UserExists(userId))
            {
                throw new NotFoundException("Invalid user Id");
            }

            if (await _matchesService.MatchExists(matchId))
            {
                throw new NotFoundException("Invalid match Id");
            }

            await _matchConnectionsService.CreateMatchConnection(userId, matchId);
        }

        public async Task<bool> UserConnectedToMatch(int userId)
        {
            return await _matchConnectionsService.UserConnectedToMatch(userId);
        }
}
