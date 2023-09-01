using AutoMapper;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.Users;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service implementation for our Matches
/// </summary>
public class MatchesService : IMatchesService
{
    private readonly IMatchesReporitory _matchesRepo;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;

    private const int TOKEN_EXPIRATION_HOURS = 5;

    public MatchesService(IMatchesReporitory matchesRepository, IConfiguration configuration, IMapper mapper, ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MatchCreationResponseDTO> CreateNewMatch(int userId)
    {

        // Map the request to a Match object
        var match = new Match()
        {
            OwnerId = userId
        };

        // Create the match
        var createdMatch = await _matchesRepo.CreateAsync(match);

        // Create the response
        var response = new MatchCreationResponseDTO
        {
            MatchId = match.Id
        };

        return response;

    }


}
