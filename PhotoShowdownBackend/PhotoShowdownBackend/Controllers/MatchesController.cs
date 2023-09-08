using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Facades.Matches;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.Session;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchesFacade _matchesFacade;
    private readonly ISessionService _sessionService;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(IMatchesFacade matchesFacade, ISessionService sessionService, ILogger<MatchesController> logger)
    {
        _matchesFacade = matchesFacade;
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
            int userId = _sessionService.GetCurrentUserId();
            var newMatchDetails = await _matchesFacade.CreateNewMatch(userId);
            response.Data = newMatchDetails;
            return StatusCode(StatusCodes.Status201Created, response);
            //return CreatedAtAction(nameof(GetUser), new { id = newUserDetails.Id }, response);
        }
        catch(UserAlreadyConnectedException ex)
        {
            return BadRequest(response.ToErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateNewMatch)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
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
            var allMatches = await _matchesFacade.GetAllOpenMatches();
            response.Data = allMatches;
            return Ok(response);
            //return CreatedAtAction(nameof(GetUser), new { id = newUserDetails.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetAllOpenMatches)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
        }

    }


    /// <summary>
    /// Return match object by given Id
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<MatchDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMatchById(int matchId)
    {
        APIResponse <MatchDTO> response = new();
        try
        {
            var match = await _matchesFacade.GetMatchById(matchId);
            response.Data = match;
            return Ok(response);
        }
        catch(NotFoundException ex)
        {
            return NotFound(response.ToErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetMatchById)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
        }

    }

}
