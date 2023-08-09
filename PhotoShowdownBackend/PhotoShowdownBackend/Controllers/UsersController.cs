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
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO userRegistrationDTO)
    {
        APIResponse<RegisterationResponseDTO> response = new();
        try
        {
            var newUser =  await _usersService.RegisterUser(userRegistrationDTO);

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

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(APIResponse<>), StatusCodes.Status201Created)]
    public async Task<IActionResult> GetUser(int id)
    {
        throw new NotImplementedException();
    }
}
