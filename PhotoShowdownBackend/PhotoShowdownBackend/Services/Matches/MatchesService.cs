using AutoMapper;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.MatchConnections;


namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service implementation for our Matches
/// </summary>
public class MatchesService : IMatchesService
{
    private readonly IMatchesReporitory _matchesRepo;
    private readonly IUsersRepository _usersRepo;
    private readonly IMatchConnectionsService _matchConnectionsService;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;


    public MatchesService(IMatchesReporitory matchesRepository, IUsersRepository usersRepository, IMatchConnectionsService matchConnectionsService, IMapper mapper, ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _usersRepo = usersRepository;
        _matchConnectionsService = matchConnectionsService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MatchCreationResponseDTO> CreateNewMatch(int userId)
    {
        if (await _usersRepo.IsConnected(userId))
        {
            throw new UserAlreadyConnectedException();
        }

        // Map the request to a Match object
        var match = new Match()
        {
            OwnerId = userId
        };

        // Create the match
        await _matchesRepo.CreateAsync(match);

        // Create the response
        var response = new MatchCreationResponseDTO
        {
            MatchId = match.Id
        };

        await _matchConnectionsService.CreateMatchConnection(userId, match.Id);

        return response;

    }
    //
    public async Task<List<MatchDTO>> GetAllOpenMatches()
    {
        List<Match> allMatches = await _matchesRepo.GetAllWithUsersAsync(match => match.StartDate == null,tracked:false);

        List<MatchDTO> matches = allMatches.Select(match => { 
            var dto =  _mapper.Map<MatchDTO>(match);
            dto.OwnerName = match.Owner.Username;
            dto.UsersNames = match.MatchConnections.Select(mc => mc.User.Username).ToList();
            return dto;
        }).ToList();


        return matches;
    }

   

}
