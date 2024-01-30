using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Session;
using PhotoShowdownBackend.Services.Users;
using PhotoShowdownBackend.Utils;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class MatchConnectionsController : ControllerBase
{
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly IUsersService _usersService;
    private readonly IMatchesService _matchesService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<MatchConnectionsController> _logger;

    public MatchConnectionsController(IMatchConnectionsService matchConnectionsService, IUsersService usersService
        , IMatchesService matchesService, ISessionService sessionService, ILogger<MatchConnectionsController> logger)
    {
        _matchConnectionsService = matchConnectionsService;
        _usersService = usersService;
        _matchesService = matchesService;
        _sessionService = sessionService;
        _logger = logger;
    }


    [HttpPost]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> JoinMatch(int userId, int matchId)
    {
        APIResponse response = new();
        try
        {
            if (!await _usersService.DoesUserExist(userId))
            {
                return NotFound(response.ErrorResponse("Invalid user Id"));
            }

            if (!await _matchesService.DoesMatchExists(matchId))
            {
               return NotFound(response.ErrorResponse("Invalid match Id"));
            }

            await _matchConnectionsService.CreateMatchConnection(userId, matchId);
            return Ok(response);
        }
        catch (UserAlreadyConnectedException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(JoinMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ServerError);
        }
    }

    [HttpDelete]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeaveMatch(int userId,int matchId)
    {
        APIResponse response = new();
        try
        {
            await _matchConnectionsService.DeleteMatchConnection(userId, matchId);

            if (await _matchConnectionsService.IsMatchEmpty(matchId))
            {
                await _matchesService.DeleteMatch(matchId);
            }
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
}
