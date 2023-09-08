using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Facades.MatchConnections;
using PhotoShowdownBackend.Facades.Matches;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Session;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class MatchConnectionsController : ControllerBase
{
    private readonly IMatchConnectionsFacade _matchConnectionsFacade;
    private readonly ISessionService _sessionService;
    private readonly ILogger<MatchConnectionsController> _logger;

    public MatchConnectionsController(IMatchConnectionsFacade matchConnectionsFacade, ISessionService sessionService, ILogger<MatchConnectionsController> logger)
    {
        _matchConnectionsFacade = matchConnectionsFacade;
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
            await _matchConnectionsFacade.CreateMatchConnection(userId,matchId);
            return Ok(response);
        }
        catch (UserAlreadyConnectedException ex)
        {
            return BadRequest(response.ToErrorResponse(ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ToErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(JoinMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeaveMatch(int userId,int matchId)
    {
        APIResponse response = new();
        try
        {
            await _matchConnectionsFacade.LeaveMatch(userId, matchId);
            return Ok(response);
        }
        catch (UserAlreadyConnectedException ex)
        {
            return BadRequest(response.ToErrorResponse(ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(response.ToErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LeaveMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
        }
    }

}
