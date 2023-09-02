using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Users;
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
    private readonly IMatchesService _matchesService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(IMatchesService matchesService, ISessionService sessionService, ILogger<MatchesController> logger)
    {
        _matchesService = matchesService;
        _sessionService = sessionService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<MatchCreationResponseDTO>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateNewMatch()
    {
        APIResponse<MatchCreationResponseDTO> response = new();
        try
        {
            int userId = _sessionService.GetCurrentUserId();
            var newMatchDetails = await _matchesService.CreateNewMatch(userId);
            response.Data = newMatchDetails;
            return StatusCode(StatusCodes.Status201Created, response);
            //return CreatedAtAction(nameof(GetUser), new { id = newUserDetails.Id }, response);
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
    public async Task<IActionResult> GetAllOpenMatches()
    {
        APIResponse<List<MatchDTO>> response = new();
        try
        {
            var allMatches = await _matchesService.GetAllOpenMatches();
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

}
