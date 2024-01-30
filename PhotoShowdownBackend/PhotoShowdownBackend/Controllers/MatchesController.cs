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
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(IMatchConnectionsService matchConnectionsService, IMatchesService matchesService, ISessionService sessionService, ILogger<MatchesController> logger)
    {
        _matchesService = matchesService;

        _matchConnectionsService = matchConnectionsService;
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

            if (await _matchConnectionsService.IsUserConnectedToMatch(ownerId))
            {
                return BadRequest(response.ErrorResponse("User is already connected to a match"));
            }

            var newMatchDetails = await _matchesService.CreateNewMatch(ownerId);
            await _matchConnectionsService.CreateMatchConnection(ownerId, newMatchDetails.Id);

            response.Data = newMatchDetails;
            //return StatusCode(StatusCodes.Status201Created, response);
            return CreatedAtAction(nameof(GetMatchById), new { id = newMatchDetails.Id }, response);
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
    [HttpGet]
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
}
