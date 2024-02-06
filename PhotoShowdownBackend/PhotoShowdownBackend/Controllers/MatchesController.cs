using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Session;
using PhotoShowdownBackend.Services.Users;
using PhotoShowdownBackend.Utils;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchesService _matchesService;
    private readonly IUsersService _usersService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(IMatchesService matchesService,IUsersService usersService, ISessionService sessionService, ILogger<MatchesController> logger)
    {
        _matchesService = matchesService;
        _usersService = usersService;
        _sessionService = sessionService;
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

            var newMatchDetails = await _matchesService.CreateNewMatch(ownerId);
            await _matchesService.CreateMatchConnection(ownerId, newMatchDetails.Id);

            response.Data = newMatchDetails;
            //return StatusCode(StatusCodes.Status201Created, response);
            return CreatedAtAction(nameof(GetMatchById), new { id = newMatchDetails.Id }, response);
        }
        catch (UserAlreadyConnectedException ex)
        {
            _logger.LogError(ex, $"{nameof(CreateNewMatch)} Error");
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateNewMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ServerError);
        }
    }


    /// <summary>
    /// Gets all open matches
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<List<MatchDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllOpenMatches()
    {
        APIResponse<List<MatchDTO>> response = new();
        try
        {
            var allMatches = await _matchesService.GetAllOpenMatches();
            response.Data = allMatches;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetAllOpenMatches)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ServerError);
        }
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
        APIResponse <MatchDTO> response = new();
        try
        {
            var match = await _matchesService.GetMatchById(matchId);
            response.Data = match;
            return Ok(response);
        }
        catch(NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetMatchById)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ServerError);
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

            if (!await _usersService.DoesUserExist(userId))
            {
                return NotFound(response.ErrorResponse("Invalid user Id"));
            }

            await _matchesService.JoinMatch(userId, matchId);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, $"{nameof(JoinMatch)} Error");
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (UserAlreadyConnectedException ex)
        {
            _logger.LogError(ex, $"{nameof(JoinMatch)} Error");
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(JoinMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ServerError);
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
            await _matchesService.LeaveMatch(userId, matchId);
           
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LeaveMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ServerError);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentMatch()
    {
        APIResponse<MatchDTO> response = new();
        try
        {
            int userId = _sessionService.GetCurrentUserId();     
            var match = await _matchesService.GetMatchById(userId);
            response.Data = match;
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LeaveMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ServerError);
        }
    }
}
