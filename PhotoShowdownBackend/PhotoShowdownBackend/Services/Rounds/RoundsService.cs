using AutoMapper;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Pictures;
using PhotoShowdownBackend.Repositories.Rounds;
using PhotoShowdownBackend.Services.CustomSentences;
using PhotoShowdownBackend.Services.Pictures;

namespace PhotoShowdownBackend.Services.Rounds;

public class RoundsService: IRoundsService
{
    private readonly IRoundsRepository _roundsRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<RoundsService> _logger;
    private readonly ISentencesService _sentencesService;

    public RoundsService(IRoundsRepository roundsRepo,IMapper mapper,ILogger<RoundsService> logger,ISentencesService customSentencesService)
    {
        _roundsRepo = roundsRepo;
        _mapper = mapper;
        _logger = logger;
        _sentencesService = customSentencesService;
    }

    public async Task<RoundDTO> StartRound(int matchId,int roundIndex)
    {

        var round = new Round()
        {
            MatchId = matchId,
            RoundIndex = roundIndex,
            StartDate = DateTime.UtcNow,
            Sentence = await _sentencesService.FetchSentence(matchId)
        };

        var roundDto = _mapper.Map<RoundDTO>(round);

        return roundDto;
    }

    public async Task<RoundDTO> EndRound(int matchId)
    {
        return null; 
    }
}
