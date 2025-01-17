﻿using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PhotoShowdownBackend.Attributes;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.Users;
using PhotoShowdownBackend.Services.Session;
using PhotoShowdownBackend.Services.Users;
using System.Net;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[HandleException]
[EnableRateLimiting("token")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUsersService usersService, ISessionService sessionService, ILogger<UsersController> logger)
    {
        _usersService = usersService;
        _sessionService = sessionService;
        _logger = logger;
    }

    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="registrationRequest">Details of created user</param>
    /// <returns>Id of created user</returns>
    [HttpPost, AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<RegisterationResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO registrationRequest)
    {
        APIResponse<RegisterationResponseDTO> response = new();
        try
        {
            var newUserDetails =  await _usersService.CreateUser(registrationRequest);

            response.Data = newUserDetails;

            return CreatedAtAction(nameof(GetUser), new { id = newUserDetails.Id }, response);
        }
        catch (UsersServiceException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <param name="loginRequest">Username and passowrd</param>
    /// <returns>JWT token for user</returns>
    [HttpPost, AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        APIResponse<LoginResponseDTO> response = new();
        try
        {
            _logger.LogInformation($"User {loginRequest.Username} logging in request");
            var loginResult = await _usersService.Login(loginRequest);

            response.Data = loginResult;

            _logger.LogInformation($"User {loginRequest.Username} logged in");
            return Ok(response);
        }
        catch (InvalidLoginException ex)
        {
            return BadRequest(response.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Returns a user by id
    /// </summary>
    /// <param name="id">Id of user</param>
    /// <returns>User DTO</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(int id)
    {
        APIResponse<UserDTO> response = new();
        try
        {
            if (id != _sessionService.GetCurrentUserId() && !_sessionService.IsCurrentUserInRole(Roles.Admin))
            {
                return StatusCode(StatusCodes.Status403Forbidden, response.ErrorResponse("Can't get user if client is not the user or a admin"));
            }
            var user = await _usersService.GetUser(id);
            if (user == null)
            {
                return NotFound(response.ErrorResponse("User not found"));
            }

            response.Data = user!;
            return Ok(response);
        }
        catch( NotFoundException ex)
        {
            return NotFound(response.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get all users (admin only)
    /// </summary>
    /// <returns>All users in the system</returns>
    [HttpGet, Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(APIResponse<IEnumerable<int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> GetAllUsers()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}
