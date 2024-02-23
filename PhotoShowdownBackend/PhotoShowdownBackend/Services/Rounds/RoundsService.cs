using AutoMapper;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.Matches;
using PhotoShowdownBackend.Exceptions.Rounds;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Pictures;
using PhotoShowdownBackend.Repositories.Rounds;
using PhotoShowdownBackend.Services.CustomSentences;
using PhotoShowdownBackend.Services.Pictures;
using System.Text.RegularExpressions;

namespace PhotoShowdownBackend.Services.Rounds;

public class RoundsService : IRoundsService
{
    private readonly IRoundsRepository _roundsRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<RoundsService> _logger;
    private readonly ISentencesService _sentencesService;

    public RoundsService(IRoundsRepository roundsRepo, IMapper mapper, ILogger<RoundsService> logger, ISentencesService customSentencesService)
    {
        _roundsRepo = roundsRepo;
        _mapper = mapper;
        _logger = logger;
        _sentencesService = customSentencesService;
    }

    public async Task<RoundDTO> StartRound(int matchId, int roundIndex)
    {
        var sentence = await _sentencesService.FetchSentence(matchId) ??
            throw new CantFetchSentenceException();

        var round = new Round()
        {
            MatchId = matchId,
            RoundIndex = roundIndex,
            StartDate = DateTime.UtcNow,
            RoundState = Round.RoundStates.PictureSelection,
            Sentence = sentence
        };

        await _roundsRepo.CreateAsync(round);

        return _mapper.Map<RoundDTO>(round);
    }

    public async Task<RoundDTO> EndRound(int matchId, int roundIndex)
    {
        var round = await _roundsRepo.GetAsync(r => r.MatchId == matchId && r.RoundIndex == roundIndex);

        if (round == null)
        {
            throw new NotFoundException("Round not found");
        }

        round.RoundState = Round.RoundStates.Ended;
        round.EndDate = DateTime.UtcNow;

        await _roundsRepo.UpdateAsync(round);

        return _mapper.Map<RoundDTO>(round);
    }

    public async Task<RoundDTO> GetCurrentRound(int matchId)
    {

        Round lastRound = await _roundsRepo.GetLast(matchId) ??
            throw new NotFoundException();
        
        RoundDTO roundDTO = _mapper.Map<RoundDTO>(lastRound);

        return roundDTO;
    }
}
