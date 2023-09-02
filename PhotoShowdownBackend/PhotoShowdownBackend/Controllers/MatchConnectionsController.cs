using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Session;

namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class MatchConnectionsController : ControllerBase
{
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<MatchConnectionsController> _logger;

    public MatchConnectionsController(IMatchConnectionsService matchConnectionsService, ISessionService sessionService, ILogger<MatchConnectionsController> logger)
    {
        _matchConnectionsService = matchConnectionsService;
        _sessionService = sessionService;
        _logger = logger;
    }


}
