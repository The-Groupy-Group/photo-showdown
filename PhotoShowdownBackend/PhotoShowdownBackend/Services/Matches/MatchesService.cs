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
    private readonly IUsersRepository _usersRepo;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;

    private const int TOKEN_EXPIRATION_HOURS = 5;

    public MatchesService(IMatchesReporitory matchesRepository, IUsersRepository usersRepo, IConfiguration configuration, IMapper mapper, ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _usersRepo = usersRepo;
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

        await JoinUserToMatch(userId, match.Id);

        return response;

    }

    public async Task<AllMatchesResponseDTO> GetAllOpenMatches()
    {
        List<Match> allMatches = await _matchesRepo.GetAllWithUsersAsync(match => match.StartDate == null,tracked:false);

        List<MatchDTO> matchesDTO = allMatches.Select(match => { 
            var dto =  _mapper.Map<MatchDTO>(match);
            dto.OwnerName = match.Users.Where(user => user.Id == match.OwnerId).First().Username;
            dto.UsersNames = match.Users.Select(user => user.Username).ToList();
            return dto;
        }).ToList();

        var response = new AllMatchesResponseDTO
        {
            Matches = allMatches
        };

        return response;

    }

    public async Task JoinUserToMatch(int userId,int matchId)
    {
        var match = await _matchesRepo.GetAsync(match => match.Id == matchId);

        if (!await _matchesRepo.DoesMatchExists(matchId))
        {
            throw new NotFoundException(/*"Match not found"*/);
        }

        var user = await _usersRepo.GetAsync(user => user.Id == userId);

        if (user == null)
        {
            throw new NotFoundException(/*"User not found"*/);
        }

        user.MatchId = matchId;
        await _usersRepo.UpdateAsync(user);


    }

}
