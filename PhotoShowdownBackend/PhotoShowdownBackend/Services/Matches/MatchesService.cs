﻿using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Utils;
using PhotoShowdownBackend.WebSockets;
using PhotoShowdownBackend.WebSockets.Messages;
using System.Text.Json;


namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service implementation for our Matches
/// </summary>
public class MatchesService : IMatchesService
{
    private readonly IMatchesReporitory _matchesRepo;
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly WebSocketRoomManager _webSocketRoomManager;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;


    public MatchesService(
        IMatchesReporitory matchesRepository,
        IMatchConnectionsService matchConnectionsService,
        WebSocketRoomManager webSocketRoomManager,
        IMapper mapper,
        ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _matchConnectionsService = matchConnectionsService;
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

        await _matchConnectionsService.CreateMatchConnection(user.Id, matchId);

        var wsMessage = new PlayerJoinedWebSocketMessage(user);
        await _webSocketRoomManager.SendMessageToRoom(user.Id, matchId, wsMessage);
    }

    public async Task RemoveUserFromMatch(UserPublicDetailsDTO user, int matchId)
    {
        await _matchConnectionsService.DeleteMatchConnection(user.Id, matchId);

        var wsMessage = new PlayerLeftWebSocketMessage(user);
        await _webSocketRoomManager.SendMessageToRoom(user.Id, matchId, wsMessage);
        await _webSocketRoomManager.CloseConnection(user.Id, matchId);

        if (await _matchConnectionsService.IsMatchEmpty(matchId))
        {
            await DeleteMatch(matchId);
        }
    }

    public async Task<bool> IsUserConnectedToMatch(int userId)
    {
        return await _matchConnectionsService.IsUserConnectedToMatch(userId);
    }

    public async Task CreateMatchConnection(int userId, int matchId)
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
}
