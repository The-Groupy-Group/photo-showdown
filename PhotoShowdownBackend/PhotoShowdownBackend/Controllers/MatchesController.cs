using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using PhotoShowdownBackend.Attributes;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Messages;
using PhotoShowdownBackend.Dtos.RoundPictures;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.RoundVotes;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Dtos.WebSocketMessages;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Exceptions.Matches;
using PhotoShowdownBackend.Exceptions.Rounds;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Session;
using PhotoShowdownBackend.Services.Users;
using PhotoShowdownBackend.WebSockets;
using System.Net.WebSockets;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[HandleException]
public class MatchesController : ControllerBase
{
    private readonly IMatchesService _matchesService;
    private readonly IUsersService _usersService;
    private readonly WebSocketRoomManager _webSocketManager;
    private readonly ISessionService _sessionService;
    private readonly IPicturesService _picturesService;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(
        IMatchesService matchesService,
        IUsersService usersService,
        WebSocketRoomManager webSocketManager,
        ISessionService sessionService,
        IPicturesService picturesService,
        ILogger<MatchesController> logger)
    {
        _matchesService = matchesService;
        _usersService = usersService;
        _webSocketManager = webSocketManager;
        _sessionService = sessionService;
        _picturesService = picturesService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<MatchCreationResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewMatch()
    {
        APIResponse<MatchCreationResponseDTO> response = new();
        try
        {
            int ownerId = _sessionService.GetCurrentUserId();
            UserPublicDetailsDTO owner = await _usersService.GetUserPublicDetails(ownerId);

            var newMatchDetails = await _matchesService.CreateNewMatch(owner);

            response.Data = newMatchDetails;

            return CreatedAtAction(nameof(this.GetMatchById), new { matchId = newMatchDetails.Id }, response);
        }
        catch (UserAlreadyConnectedException ex)
        {
            _logger.LogError(ex, $"{nameof(CreateNewMatch)} Error");
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }


    /// <summary>
    /// Gets all open matches
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<List<MatchDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllMatches(MatchStates? state)
    {
        APIResponse<List<MatchDTO>> response = new();

        var allMatches = await _matchesService.GetAllMatches(state);
        response.Data = allMatches;
        return Ok(response);
    }


    /// <summary>
    /// Return match object by given Id
    /// </summary>
    /// <returns></returns>
    [HttpGet("{matchId:int}")]
    [ProducesResponseType(typeof(APIResponse<MatchDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMatchById(int matchId)
    {
        APIResponse<MatchDTO> response = new();
        try
        {
            var match = await _matchesService.GetMatchById(matchId);
            response.Data = match;
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("{matchId:int}")]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> JoinMatch(int matchId)
    {
        APIResponse response = new();
        try
        {
            int userId = _sessionService.GetCurrentUserId();
            UserPublicDetailsDTO user = await _usersService.GetUserPublicDetails(userId);

            if (!await _usersService.DoesUserExist(userId))
            {
                return NotFound(response.ErrorResponse("Invalid user Id"));
            }

            await _matchesService.AddUserToMatch(user, matchId);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (UserAlreadyConnectedException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (MatchAlreadyStartedException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{matchId:int}")]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeaveMatch(int matchId)
    {
        APIResponse response = new();
        try
        {
            int userId = _sessionService.GetCurrentUserId();
            UserPublicDetailsDTO user = await _usersService.GetUserPublicDetails(userId);

            await _matchesService.RemoveUserFromMatch(user, matchId);

            return Ok(response);
        }
        catch (UserAlreadyConnectedException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<MatchDTO?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentMatch()
    {
        APIResponse<MatchDTO?> response = new();
        int userId = _sessionService.GetCurrentUserId();
        var match = await _matchesService.GetMatchByUserId(userId);
        response.Data = match;
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartMatch([FromBody] StartMatchDTO startMatchDTO)
    {
        APIResponse response = new();
        try
        {
            int userId = _sessionService.GetCurrentUserId();
            await _matchesService.StartMatch(userId, startMatchDTO);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (MatchAlreadyStartedException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (UserIsNotMatchOwnerException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<RoundDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentRound(int matchId)
    {
        APIResponse<RoundDTO> response = new();
        try
        {
            RoundDTO roundDTO = await _matchesService.GetCurrentRound(matchId);
            response.Data = roundDTO;
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (MatchDidNotStartYetException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SelectPictureForRound(SelectPictureForRoundRequestDTO request)
    {
        APIResponse response = new();
        try
        {
            int userId = _sessionService.GetCurrentUserId();
            await _matchesService.SelectPictureForRound(request.PictureId, request.MatchId, request.RoundIndex, userId);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (MatchDidNotStartYetException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (RoundNotInSelectionStateException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VoteForSelectedPicture(VoteForSelectedPictureRequestDTO request)
    {
        APIResponse response = new();
        try
        {
            int userId = _sessionService.GetCurrentUserId();
            await _matchesService.VoteForSelectedPicture(request.RoundPictureId, request.MatchId, request.RoundIndex, userId);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (MatchDidNotStartYetException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (RoundNotInVotingStateException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }

}
