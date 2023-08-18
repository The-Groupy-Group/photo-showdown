using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Session;


namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PicturesController : ControllerBase
{
    private readonly IPicturesService _picturesService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<UsersController> _logger;

    public PicturesController(IPicturesService picturesService, ISessionService sessionService, ILogger<UsersController> logger)
    {
        _picturesService = picturesService;
        _sessionService = sessionService;
        _logger = logger;
    }
}
