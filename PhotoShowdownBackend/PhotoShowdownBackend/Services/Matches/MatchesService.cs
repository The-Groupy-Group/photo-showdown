using AutoMapper;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.MatchConnections;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.MatchConnections;


namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service implementation for our Matches
/// </summary>
public class MatchesService : IMatchesService
{
    private readonly IMatchesReporitory _matchesRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchesService> _logger;


    public MatchesService(IMatchesReporitory matchesRepository, IMapper mapper, ILogger<MatchesService> logger)
    {
        _matchesRepo = matchesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MatchCreationResponseDTO> CreateNewMatch(int ownerId)
    {
        // Map the request to a Match object
        var match = new Match()
        {
            OwnerId = ownerId
        };

        // Create the match
        await _matchesRepo.CreateAsync(match);

        // Create the response
        var response = _mapper.Map<MatchCreationResponseDTO>(match);

        return response;
    }
    
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

    public async Task<bool> DoesMatchExists(int matchId)
    {
        return await _matchesRepo.AnyAsync(match => match.Id == matchId);
    }

    public async Task DeleteMatch(int matchId)
    {
        Match? m = await _matchesRepo.GetAsync(m => m.Id == matchId);

        if (m == null)
        {
            throw new NotFoundException("Match not found");
        }

        await _matchesRepo.DeleteAsync(m);
    }

    public async Task<MatchDTO> GetMatchById(int matchId)
    {
        Match? match = await _matchesRepo.GetWithUsersAsync(m => m.Id == matchId,false) ?? throw new NotFoundException("Invalid match Id");
        MatchDTO matchDTO = new MatchDTO()
        { 
            Id = matchId,
            OwnerName = match.Owner.Username,
            UsersNames = match.MatchConnections.Select(mc => mc.User.Username).ToList()
        };
        return matchDTO;


    }

}
