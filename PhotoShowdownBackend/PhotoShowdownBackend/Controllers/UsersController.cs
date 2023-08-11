using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions.Users;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Services.Users;
using System.Net;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUsersService usersService, ILogger<UsersController> logger)
    {
        _usersService = usersService;
        _logger = logger;
    }

    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="registrationRequest">Details of created user</param>
    /// <returns>Id of created user</returns>
    [HttpPost, AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<RegisterationResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO registrationRequest)
    {
        APIResponse<RegisterationResponseDTO> response = new();
        try
        {
            var newUserDetails =  await _usersService.RegisterUser(registrationRequest);

            response.Data = newUserDetails;

            return CreatedAtAction(nameof(GetUser), new { id = newUserDetails.Id }, response);
        }
        catch (UsersServiceException ex)
        {
            return BadRequest(response.ToErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(Register)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, EmptyAPIResponse.ToServerError());
        }
    }

    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <param name="loginRequest">Username and passowrd</param>
    /// <returns>JWT token for user</returns>
    [HttpPost, AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        APIResponse<LoginResponseDTO> response = new();
        try
        {
            _logger.LogDebug("Donfil");
            _logger.LogInformation($"User {loginRequest.Username} logging in request");
            var loginResult = await _usersService.Login(loginRequest);

            response.Data = loginResult;

            _logger.LogInformation($"User {loginRequest.Username} logged in");
            return Ok(response);
        }
        catch (InvalidLoginException ex)
        {
            return BadRequest(response.ToErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(Login)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, EmptyAPIResponse.ToServerError());
        }
    }

    /// <summary>
    /// Returns a user by id
    /// </summary>
    /// <param name="id">Id of user</param>
    /// <returns>User DTO</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(APIResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(int id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, EmptyAPIResponse.ToServerError());
    }

    /// <summary>
    /// Get all users (admin only)
    /// </summary>
    /// <returns>All users in the system</returns>
    [HttpGet, Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(APIResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUser()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, EmptyAPIResponse.ToServerError());
    }
}
