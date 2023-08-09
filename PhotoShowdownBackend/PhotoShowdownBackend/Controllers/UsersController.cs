using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<RegisterationResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO registrationRequest)
    {
        APIResponse<RegisterationResponseDTO> response = new();
        try
        {
            var newUser =  await _usersService.RegisterUser(registrationRequest);

            response.Data = newUser;

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, response);
        }
        catch (UsersServiceException ex)
        {
            return BadRequest(response.ToErrorResponse(ex.Message));
        }
        catch
        {
            // TODO: Log exception
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse<RegisterationResponseDTO>.ToServerError());
        }
    }


    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyAPIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        APIResponse<LoginResponseDTO> response = new();
        try
        {
            var loginResult = await _usersService.Login(loginRequest);

            response.Data = loginResult;

            return Ok(response);
        }
        catch (InvalidLoginException ex)
        {
            return BadRequest(response.ToErrorResponse(ex.Message));
        }
        catch
        {
            // TODO: Log exception
            return StatusCode(StatusCodes.Status500InternalServerError, EmptyAPIResponse.ToServerError());
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(APIResponse<>), StatusCodes.Status201Created)]
    public async Task<IActionResult> GetUser(int id)
    {
        throw new NotImplementedException();
    }
}
