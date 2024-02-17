using AutoMapper;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Messages;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Dtos.WebSocketMessages;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Exceptions.Matches;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Rounds;
using PhotoShowdownBackend.WebSockets;



namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service implementation for our Matches
/// </summary>
public class MatchesService : IMatchesService
{
    private readonly IMatchesReporitory _matchesRepo;
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly IRoundsService _roundsService;
    private readonly WebSocketRoomManager _webSocketRoomManager;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;
    private const int ROUND_WINNER_DISPLAY_SECONDS = 15;

    public MatchesService(
        IMatchesReporitory matchesRepository,
        IMatchConnectionsService matchConnectionsService,
        IRoundsService roundsService,
        WebSocketRoomManager webSocketRoomManager,
        IMapper mapper,
        ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _matchConnectionsService = matchConnectionsService;
        _roundsService = roundsService;
        _webSocketRoomManager = webSocketRoomManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MatchCreationResponseDTO> CreateNewMatch(int ownerId)
    {
        if (await _matchConnectionsService.IsUserConnectedToMatch(ownerId))
        {
            throw new UserAlreadyConnectedException();
        }

        // Map the request to a Match object
        var match = new Match()
        {
            OwnerId = ownerId
        };

        // Create the match
        await _matchesRepo.CreateAsync(match);

        // Create the response
        var response = _mapper.Map<MatchCreationResponseDTO>(match);

        return response;
    }

    public async Task<List<MatchDTO>> GetAllOpenMatches()
    {
        List<Match> allMatches = await _matchesRepo.GetAllWithUsersAsync(match => match.StartDate == null);

        List<MatchDTO> matches = allMatches.Select(match =>
        {
            var dto = _mapper.Map<MatchDTO>(match);
            return dto;
        }).ToList();

        return matches;
    }

    public async Task<bool> DoesMatchExists(int matchId)
    {
        return await _matchesRepo.AnyAsync(match => match.Id == matchId);
    }

    public async Task DeleteMatch(int matchId)
    {
        Match? m = await _matchesRepo.GetAsync(m => m.Id == matchId);

        if (m == null)
        {
            throw new NotFoundException("Match not found");
        }

        await _matchesRepo.DeleteAsync(m);
    }

    public async Task<MatchDTO> GetMatchById(int matchId)
    {
        Match? match = await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId) ?? throw new NotFoundException("Invalid match Id");
        MatchDTO matchDTO = _mapper.Map<MatchDTO>(match);
        return matchDTO;
    }

    public async Task AddUserToMatch(UserPublicDetailsDTO user, int matchId)
    {
        if (!await DoesMatchExists(matchId))
        {
            throw new NotFoundException(matchId);
        }
        // TODO: Swap with m.HasMatchStarted if possible
        bool hasMatchStarted = await _matchesRepo
            .AnyAsync(m => m.Id == matchId && m.StartDate != null && DateTime.UtcNow >= m.StartDate);
        if (hasMatchStarted)
        {
            throw new MatchAlreadyStartedException();
        }

        await _matchConnectionsService.CreateMatchConnection(user.Id, matchId);

        var wsMessage = new PlayerJoinedWebSocketMessage(user);
        await _webSocketRoomManager.SendMessageToRoom(user.Id, matchId, wsMessage);
    }

    public async Task RemoveUserFromMatch(UserPublicDetailsDTO userToRemove, int matchId)
    {
        // Delete the connection
        await _matchConnectionsService.DeleteMatchConnection(userToRemove.Id, matchId);

        // If the match is empty, delete it
        if (await _matchConnectionsService.IsMatchEmpty(matchId))
        {
            await DeleteMatch(matchId);
            return;
        }

        // Send a message to the room
        var playerLeftWsMessage = new PlayerLeftWebSocketMessage(userToRemove);
        await _webSocketRoomManager.SendMessageToRoom(userToRemove.Id, matchId, playerLeftWsMessage);
        await _webSocketRoomManager.CloseConnection(userToRemove.Id, matchId);

        Match match = (await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId))!;
        // If the leaving user is the owner, assign a new owner
        if (match.OwnerId == userToRemove.Id)
        {
            var newOwner = match.MatchConnections.First().User;
            match.OwnerId = newOwner.Id;
            await _matchesRepo.UpdateAsync(match);

            // Send a message to the room
            var newOwnerWsMessage = new NewOwnerWebSocketMessage(_mapper.Map<UserPublicDetailsDTO>(newOwner));
            await _webSocketRoomManager.SendMessageToRoom(userToRemove.Id, matchId, newOwnerWsMessage);
        }
    }

    public async Task<bool> IsUserConnectedToMatch(int userId)
    {
        return await _matchConnectionsService.IsUserConnectedToMatch(userId);
    }

    public async Task ConnectUserToMatch(int userId, int matchId)
    {
        await _matchConnectionsService.CreateMatchConnection(userId, matchId);
    }

    public async Task<MatchDTO> GetMatchByUserId(int userId)
    {
        int matchId = await _matchConnectionsService.GetMatchIdByUserId(userId);
        Match match = (await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId))!;

        MatchDTO matchDTO = _mapper.Map<MatchDTO>(match);
        return matchDTO;
    }

    public async Task StartMatch(int userId, StartMatchDTO startMatchDTO)
    {
        Match match = await _matchesRepo.GetWithUsersAsync(m => m.Id == startMatchDTO.MatchId) ??
            throw new NotFoundException();

        if (match.OwnerId != userId)
        {
            throw new UserIsNotMatchOwnerException();
        }

        if (match.StartDate != null)
        {
            throw new MatchAlreadyStartedException();
        }

        // Map the request to update the Match object
        //match.StartDate = DateTime.UtcNow;
        if (startMatchDTO.PictureSelectionTimeSeconds.HasValue)
        {
            match.PictureSelectionTimeSeconds = startMatchDTO.PictureSelectionTimeSeconds.Value;
        }
        if (startMatchDTO.VoteTimeSeconds.HasValue)
        {
            match.VoteTimeSeconds = startMatchDTO.VoteTimeSeconds.Value;
        }
        if (startMatchDTO.NumOfVotesToWin.HasValue)
        {
            match.NumOfVotesToWin = startMatchDTO.NumOfVotesToWin.Value;
        }
        if (startMatchDTO.NumOfRounds.HasValue)
        {
            match.NumOfRounds = startMatchDTO.NumOfRounds.Value;
        }
        // Update the match
        await _matchesRepo.UpdateAsync(match);

        // Start the match logic
        // Get the match from the database to avoid concurrency issues
        match = (await _matchesRepo.GetAsync(m => m.Id == startMatchDTO.MatchId, tracked: false))!;

        MatchStartedWebSocketMessage matchStartedWsMessage = new();
        await _webSocketRoomManager.SendMessageToRoom(null, match.Id, matchStartedWsMessage);
        _ = Task.Run(() => ExecuteMatchLogic(match));
    }

    private async void ExecuteMatchLogic(Match match)
    {
        int roundIndex = 0;
        while (!(false/*match.NumOfRounds == roundIndex || match.NumOfVotesToWin == userWithMaxVotes*/)) // Check winning condition
        {
            // ------- Start a new round ------- //
            //var roundDto = _roundsService.StartRound(match.Id, roundIndex);
            var roundDto = new RoundDTO()
            {
                MatchId = match.Id,
                RoundIndex = 0,
                RoundState = Round.RoundStates.PictureSelection,
                StartDate = DateTime.UtcNow,
                Sentence = "PITOM DONFIL HEFLITZ TUUUM TUUUM TUTUUTUTUMMM " + roundIndex
            };
            NewRoundStartedWebSocketMessage newRoundWsMessage = new(roundDto);
            await _webSocketRoomManager.SendMessageToRoom(null, match.Id, newRoundWsMessage);
            Thread.Sleep(match.PictureSelectionTimeSeconds * 1000);

            // ------- Start voting phase ------- //
            Thread.Sleep(match.VoteTimeSeconds * 1000);
            // TODO: Implement voting logic

            // ------- Ending a round ------- //
            //roundDto = _roundsService.EndRound(match.Id, roundIndex);
            Thread.Sleep(ROUND_WINNER_DISPLAY_SECONDS * 1000);
            roundIndex++;
        }
    }
}
