﻿using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Messages;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Dtos.WebSocketMessages;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Exceptions.Matches;
using PhotoShowdownBackend.Exceptions.Rounds;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;
    private const int ROUND_WINNER_DISPLAY_SECONDS = SystemSettings.ROUND_WINNER_DISPLAY_SECONDS;

    public MatchesService(
        IMatchesReporitory matchesRepository,
        IMatchConnectionsService matchConnectionsService,
        IRoundsService roundsService,
        WebSocketRoomManager webSocketRoomManager,
        IServiceProvider serviceProvider,
        IMapper mapper,
        ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _matchConnectionsService = matchConnectionsService;
        _roundsService = roundsService;
        _webSocketRoomManager = webSocketRoomManager;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<MatchDTO>> GetAllMatches(MatchStates? state)
    {
        List<Match> allMatches = await _matchesRepo
            .GetAllWithUsersAsync(match =>
                (state.HasValue &&
                (state == MatchStates.Ended && match.EndDate >= DateTime.UtcNow) ||
                (state == MatchStates.InProgress && match.StartDate >= DateTime.UtcNow && !match.EndDate.HasValue) ||
                (state == MatchStates.NotStarted && (!match.StartDate.HasValue || match.StartDate > DateTime.UtcNow))),
                tracked: true);


        List<MatchDTO> matches = allMatches.Select(_mapper.Map<MatchDTO>).ToList();

        return matches;
    }

    public async Task<MatchDTO> GetMatchById(int matchId)
    {
        Match? match = await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId, tracked: false) ??
            throw new NotFoundException("Invalid match Id");

        MatchDTO matchDTO = _mapper.Map<MatchDTO>(match);
        return matchDTO;
    }

    public async Task<MatchDTO?> GetMatchByUserId(int userId)
    {
        int? matchId = await _matchConnectionsService.GetMatchIdByUserId(userId);
        if (matchId == null)
        {
            return null;
        }
        Match? match = (await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId, tracked: false))!;
        MatchDTO matchDTO = _mapper.Map<MatchDTO>(match);
        return matchDTO;
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

    public async Task AddUserToMatch(UserPublicDetailsDTO user, int matchId)
    {
        if (!await DoesMatchExists(matchId))
        {
            throw new NotFoundException(matchId);
        }
        // TODO: Swap with m.HasMatchStarted if possible
        bool hasMatchStarted = await _matchesRepo
            .AnyAsync(m =>
                m.Id == matchId &&
                m.StartDate != null &&
                DateTime.UtcNow >= m.StartDate);
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
        Match match = (await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId, tracked: true))!;

        // Delete the connection
        await _matchConnectionsService.DeleteMatchConnection(userToRemove.Id, matchId);

        // If the match is empty and hasent started, delete it
        bool isMatchEmpty = await _matchConnectionsService.IsMatchEmpty(matchId);
        if (!(DateTime.UtcNow > match.StartDate) && isMatchEmpty)
        {
            await DeleteMatch(matchId);
            return;
        }
        else if (isMatchEmpty) // If the match is empty and has started, end the match
        {
            match.EndDate = DateTime.UtcNow;
            // TODO: Implement the logic to end the match
        }

        // Send a message to the room
        var playerLeftWsMessage = new PlayerLeftWebSocketMessage(userToRemove);
        await _webSocketRoomManager.SendMessageToRoom(userToRemove.Id, matchId, playerLeftWsMessage);
        await _webSocketRoomManager.CloseConnection(userToRemove.Id, matchId);


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

        // Update the Match from the request
        match.StartDate = DateTime.UtcNow;
        match.PictureSelectionTimeSeconds = startMatchDTO.PictureSelectionTimeSeconds;
        match.VoteTimeSeconds = startMatchDTO.VoteTimeSeconds;
        match.NumOfVotesToWin = startMatchDTO.NumOfVotesToWin;
        match.NumOfRounds = startMatchDTO.NumOfRounds;

        await _matchesRepo.UpdateAsync(match);

        // Start the match logic
        // Get the match from the database to avoid concurrency issues
        match = (await _matchesRepo.GetAsync(m => m.Id == startMatchDTO.MatchId, tracked: false))!;

        MatchStartedWebSocketMessage matchStartedWsMessage = new();
        await _webSocketRoomManager.SendMessageToRoom(null, match.Id, matchStartedWsMessage);
        _ = Task.Run(() => ExecuteMatchLogic(match));
    }

    public async Task<RoundDTO> GetCurrentRound(int matchId)
    {
        Match match = await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId) ??
             throw new NotFoundException();

        if(match.StartDate == null || DateTime.UtcNow < match.StartDate)
            throw new MatchDidNotStartYetException();

        RoundDTO roundDTO = await _roundsService.GetCurrentRound(matchId);

        return roundDTO;
    }

    // ------------ Private methods ------------ //
    private async Task ExecuteMatchLogic(Match match)
    {
        using var scope = _serviceProvider.CreateScope();
        var roundsService = scope.ServiceProvider.GetRequiredService<IRoundsService>();
        int roundIndex = 0;
        while (!(false/*match.NumOfRounds == roundIndex || match.NumOfVotesToWin == userWithMaxVotes*/)) // Check winning condition
        {
            // ------- Start a new round ------- //
            RoundDTO roundDto;
            try
            {
                roundDto = await roundsService.StartRound(match.Id, roundIndex);
            }
            catch(CantFetchSentenceException)
            {
                // TODO: end the match prematurely
                _logger.LogError("Cant fetch sentence for match {matchId}", match.Id);
                break;
            }
            RoundStateChangeWebSocketMessage roundWsMessage = new(roundDto);
            await _webSocketRoomManager.SendMessageToRoom(null, match.Id, roundWsMessage);
            await Task.Delay(match.PictureSelectionTimeSeconds * 100099999);

            // ------- Start voting phase ------- //
            // TODO: Implement voting logic
            roundWsMessage.Data.RoundState = Round.RoundStates.Voting;
            await _webSocketRoomManager.SendMessageToRoom(null, match.Id, roundWsMessage);
            await Task.Delay(match.VoteTimeSeconds * 1000);

            // ------- Ending a round ------- //
            //roundDto = _roundsService.EndRound(match.Id, roundIndex);
            roundWsMessage.Data.RoundState = Round.RoundStates.Ended;
            await _webSocketRoomManager.SendMessageToRoom(null, match.Id, roundWsMessage);
            // TODO: Implement round winner logic
            await Task.Delay(ROUND_WINNER_DISPLAY_SECONDS * 1000);
            roundIndex++;
        }
        // TODO: Implement match end logic (send a message to the room, remove all connection)
    }

    private async Task DeleteMatch(int matchId)
    {
        Match? m = await _matchesRepo.GetAsync(m => m.Id == matchId);

        if (m == null)
        {
            throw new NotFoundException("Match not found");
        }

        await _matchesRepo.DeleteAsync(m);
    }

    private async Task<bool> IsUserConnectedToMatch(int userId)
    {
        return await _matchConnectionsService.IsUserConnectedToMatch(userId);
    }

    private async Task<bool> DoesMatchExists(int matchId)
    {
        return await _matchesRepo.AnyAsync(match => match.Id == matchId);
    }


}
