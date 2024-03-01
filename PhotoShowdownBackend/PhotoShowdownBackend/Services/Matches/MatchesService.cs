using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Messages;
using PhotoShowdownBackend.Dtos.RoundPictures;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Dtos.WebSocketMessages;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Exceptions.Matches;
using PhotoShowdownBackend.Exceptions.Rounds;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.RoundPictures;
using PhotoShowdownBackend.Repositories.RoundVotes;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.CustomSentences;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Rounds;
using PhotoShowdownBackend.WebSockets;
using System.Collections.Concurrent;



namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service implementation for our Matches
/// </summary>
public class MatchesService : IMatchesService
{
    private readonly IMatchesRepository _matchesRepo;
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly IRoundsService _roundsService;
    private readonly ISentencesService _sentencesService;
    private readonly WebSocketRoomManager _webSocketRoomManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;

    private const int ROUND_WINNER_DISPLAY_SECONDS = SystemSettings.ROUND_WINNER_DISPLAY_SECONDS;
    private static readonly ConcurrentDictionary<int, CancellationTokenSource> _cancelationTokens = new();

    public MatchesService(
        IMatchesRepository matchesRepository,
        IMatchConnectionsService matchConnectionsService,
        IRoundsService roundsService,
        ISentencesService sentencesService,
        WebSocketRoomManager webSocketRoomManager,
        IServiceProvider serviceProvider,
        IMapper mapper,
        ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _matchConnectionsService = matchConnectionsService;
        _roundsService = roundsService;
        _sentencesService = sentencesService;
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
        matchDTO.Round = await _roundsService.GetCurrentRound(matchId.Value);
        return matchDTO;
    }

    public async Task<MatchCreationResponseDTO> CreateNewMatch(UserInMatchDTO owner)
    {
        if (await _matchConnectionsService.IsUserConnectedToMatch(owner.Id))
        {
            throw new UserAlreadyConnectedException();
        }

        // Map the request to a Match object
        var match = new Match()
        {
            OwnerId = owner.Id
        };

        // Create the match
        await _matchesRepo.CreateAsync(match);

        await AddUserToMatch(owner, match.Id);

        // Create the response
        var response = _mapper.Map<MatchCreationResponseDTO>(match);

        return response;
    }

    public async Task AddUserToMatch(UserInMatchDTO user, int matchId)
    {
        if (!await DoesMatchExists(matchId))
        {
            throw new NotFoundException(matchId);
        }

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

    public async Task RemoveUserFromMatch(UserInMatchDTO userToRemove, int matchId)
    {
        // Delete the connection
        await _matchConnectionsService.DeleteMatchConnection(userToRemove.Id, matchId);

        // If the match is empty and hasent started, delete it
        Match match = (await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId, tracked: true))!;
        bool isMatchEmpty = await _matchConnectionsService.IsMatchEmpty(matchId);
        if (!(DateTime.UtcNow > match.StartDate) && isMatchEmpty)
        {
            await DeleteMatch(matchId);
            await _webSocketRoomManager.CloseRoom(matchId);
            return;
        }
        else if (isMatchEmpty) // If the match is empty and has started, end the match
        {
            await EndMatch(matchId);
            return;
        }

        // Send a message to the room
        var playerLeftWsMessage = new PlayerLeftWebSocketMessage(userToRemove);
        await _webSocketRoomManager.SendMessageToRoom(userToRemove.Id, matchId, playerLeftWsMessage);
        await _webSocketRoomManager.RemoveSocket(userToRemove.Id, matchId);


        // If the leaving user is the owner, assign a new owner
        if (match.OwnerId == userToRemove.Id)
        {
            var newOwner = match.MatchConnections.First().User;
            match.OwnerId = newOwner.Id;
            await _matchesRepo.UpdateAsync(match);

            // Send a message to the room
            var newOwnerWsMessage = new NewOwnerWebSocketMessage(_mapper.Map<UserInMatchDTO>(newOwner));
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

        // Set the custom sentences
        if (startMatchDTO.Sentences.Count > 0)
        {
            await _sentencesService.SetCustomSentences(startMatchDTO.Sentences, match.Id);
        }

        // Start the match logic
        // Get the match from the database to avoid concurrency issues
        match = (await _matchesRepo.GetAsync(m => m.Id == startMatchDTO.MatchId, tracked: false))!;

        MatchStartedWebSocketMessage matchStartedWsMessage = new();
        await _webSocketRoomManager.SendMessageToRoom(null, match.Id, matchStartedWsMessage);

        CancellationTokenSource cancellationTokenSource = new();
        _cancelationTokens[match.Id] = cancellationTokenSource;
        _ = Task.Run(() => ExecuteMatchLogic(match, _serviceProvider.CreateScope(), cancellationTokenSource.Token));
    }

    public async Task EndMatch(int matchId)
    {
        Match match = await _matchesRepo.GetAsync(m => m.Id == matchId) ??
            throw new NotFoundException();

        if (match.EndDate != null)
        {
            throw new MatchAlreadyEndedException();
        }
        // TODO: Send a message to the room https://groupy-group.atlassian.net/browse/PHSH-142

        // End the match
        match.EndDate = DateTime.UtcNow;
        await _matchesRepo.UpdateAsync(match);

        await _matchConnectionsService.DeleteAllMatchConnections(matchId);
        await _webSocketRoomManager.CloseRoom(matchId);

        if (_cancelationTokens.TryGetValue(matchId, out CancellationTokenSource? cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            _cancelationTokens.TryRemove(matchId, out _);
        }
    }

    public async Task<RoundDTO> GetCurrentRound(int matchId)
    {
        Match match = await _matchesRepo.GetAsync(m => m.Id == matchId) ??
             throw new NotFoundException();

        if (match.StartDate == null || DateTime.UtcNow < match.StartDate)
            throw new MatchDidNotStartYetException();

        RoundDTO roundDTO = await _roundsService.GetCurrentRound(matchId);

        return roundDTO;
    }

    public async Task SelectPictureForRound(int pictureId, int matchId, int roundIndex, int userId)
    {
        Match match = await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId) ??
             throw new NotFoundException();

        if (match.StartDate == null || DateTime.UtcNow < match.StartDate)
            throw new MatchDidNotStartYetException();

        await _roundsService.SelectPicture(pictureId, matchId, roundIndex, userId);

        UserLockedInWebSocketMessage userLockedInWsMessage = new(userId);
        await _webSocketRoomManager.SendMessageToRoom(userId, match.Id, userLockedInWsMessage);
    }

    public async Task VoteForSelectedPicture(int roundPictureId, int matchId, int roundIndex, int userId)
    {
        Match match = await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId) ??
             throw new NotFoundException();

        if (match.StartDate == null || DateTime.UtcNow < match.StartDate)
            throw new MatchDidNotStartYetException();

        await _roundsService.VoteForSelectedPicture(roundPictureId, matchId, roundIndex, userId);

        UserLockedInWebSocketMessage userLockedInWsMessage = new(userId);
        await _webSocketRoomManager.SendMessageToRoom(userId, match.Id, userLockedInWsMessage);
    }

    // ------------ Private methods ------------ //
    private static async Task ExecuteMatchLogic(Match match, IServiceScope scope, CancellationToken cancellationToken)
    {
        // Get the services
        var roundsService = scope.ServiceProvider.GetRequiredService<IRoundsService>();
        var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
        var webSocketRoomManager = scope.ServiceProvider.GetRequiredService<WebSocketRoomManager>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MatchesService>>();
        try
        {

            int roundIndex = 0;
            while (!(match.NumOfRounds == roundIndex/* || match.NumOfVotesToWin == userWithMaxVotes*/)) // Check winning condition
            {
                // ------- Start a new round ------- //
                RoundDTO roundDto;
                try
                {
                    roundDto = await roundsService.StartRound(match.Id, roundIndex);
                }
                catch (CantFetchSentenceException)
                {
                    logger.LogInformation("Cant fetch sentence for match {matchId}", match.Id);
                    break;
                }
                RoundStateChangeWebSocketMessage roundWsMessage = new(roundDto);
                await webSocketRoomManager.SendMessageToRoom(null, match.Id, roundWsMessage);
                await Task.Delay(match.PictureSelectionTimeSeconds * 1000, cancellationToken);

                // ------- Start voting phase ------- //
                roundDto = await roundsService.StartVotePhase(match.Id, roundIndex);
                roundWsMessage.Data = roundDto;
                await webSocketRoomManager.SendMessageToRoom(null, match.Id, roundWsMessage);
                await Task.Delay(match.VoteTimeSeconds * 1000, cancellationToken);

                // ------- Ending a round ------- //
                roundDto = await roundsService.EndRound(match.Id, roundIndex);
                roundWsMessage.Data = roundDto;
                await webSocketRoomManager.SendMessageToRoom(null, match.Id, roundWsMessage);

                await Task.Delay(ROUND_WINNER_DISPLAY_SECONDS * 1000, cancellationToken);
                roundIndex++;

                // Check if the match was ended
                cancellationToken.ThrowIfCancellationRequested();
            }
            await matchesService.EndMatch(match.Id);
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
            logger.LogInformation("ExecuteMatchLogic was canceled by CancellationToken for match {matchId}", match.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while executing match logic for match {matchId}", match.Id);
            await matchesService.EndMatch(match.Id);
        }
        finally
        {
            scope.Dispose();
        }
    }

    private async Task DeleteMatch(int matchId)
    {
        Match? m = await _matchesRepo.GetAsync(m => m.Id == matchId) ??
            throw new NotFoundException("Match not found");

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
