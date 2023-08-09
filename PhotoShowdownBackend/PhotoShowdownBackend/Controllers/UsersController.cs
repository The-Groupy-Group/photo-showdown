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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO userRegistrationDTO)
    {
        try
        {
            var newUser =  await _usersService.RegisterUser(userRegistrationDTO);

            // TODO: Change action to nameof(GetUser)
            return CreatedAtAction("", new { id = newUser.Id }, newUser);
        }
        catch (UserRegistrationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
