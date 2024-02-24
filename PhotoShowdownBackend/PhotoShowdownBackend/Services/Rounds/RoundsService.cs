using AutoMapper;
using PhotoShowdownBackend.Dtos.RoundPictures;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Exceptions.Matches;
using PhotoShowdownBackend.Exceptions.Rounds;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Pictures;
using PhotoShowdownBackend.Repositories.RoundPictures;
using PhotoShowdownBackend.Repositories.Rounds;
using PhotoShowdownBackend.Repositories.RoundVotes;
using PhotoShowdownBackend.Services.CustomSentences;
using PhotoShowdownBackend.Services.Pictures;

namespace PhotoShowdownBackend.Services.Rounds;

public class RoundsService : IRoundsService
{
    private readonly IRoundsRepository _roundsRepo;
    private readonly IRoundPicturesRepository _roundPicturesRepository;
    private readonly IRoundVotesRepository _roundVotesRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoundsService> _logger;
    private readonly ISentencesService _sentencesService;

    public RoundsService(
        IRoundsRepository roundsRepo,
                IRoundPicturesRepository roundPicturesRepository,
                IRoundVotesRepository roundVotesRepository,
        IMapper mapper,
        ILogger<RoundsService> logger,
        ISentencesService customSentencesService)
    {
        _roundsRepo = roundsRepo;
        _roundPicturesRepository = roundPicturesRepository;
        _roundVotesRepository = roundVotesRepository;
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

        round = await _roundsRepo.GetWithIncludesAsync(r => r.MatchId == matchId && r.RoundIndex == roundIndex);

        return _mapper.Map<RoundDTO>(round);
    }

    public async Task<RoundDTO> StartVotePhase(int matchId, int roundIndex)
    {
        var round = await _roundsRepo.GetWithIncludesAsync(r => r.MatchId == matchId && r.RoundIndex == roundIndex) ??
            throw new NotFoundException("Round not found");
        
        round.RoundState = Round.RoundStates.Voting;

        await _roundsRepo.UpdateAsync(round);

        return _mapper.Map<RoundDTO>(round);

    }

    public async Task<RoundDTO> EndRound(int matchId, int roundIndex)
    {
        var round = await _roundsRepo.GetWithIncludesAsync(r => r.MatchId == matchId && r.RoundIndex == roundIndex) ??
            throw new NotFoundException("Round not found");

        round.RoundState = Round.RoundStates.Ended;
        round.EndDate = DateTime.UtcNow;
        round.WinnerId = CalculateRoundWinner(round);

        await _roundsRepo.UpdateAsync(round);

        return _mapper.Map<RoundDTO>(round);
    }

    public async Task<RoundDTO> GetCurrentRound(int matchId)
    {
        Round lastRound = await _roundsRepo.GetLastWithInclude(matchId) ??
            throw new NotFoundException();

        RoundDTO roundDTO = _mapper.Map<RoundDTO>(lastRound);

        return roundDTO;
    }

    public async Task SelectPicture(int pictureId, int matchId, int roundIndex, int userId)
    {
        bool isRoundInSelectionState = await _roundsRepo
            .AnyAsync(r =>
                r.MatchId == matchId &&
                r.RoundIndex == roundIndex &&
                r.RoundState == Round.RoundStates.PictureSelection);
        if (!isRoundInSelectionState)
        {
            throw new RoundNotInSelectionStateException();
        }
        // Convert to roundpicture
        RoundPicture roundPicture = new()
        {
            PictureId = pictureId,
            UserId = userId,
            MatchId = matchId,
            RoundIndex = roundIndex,
        };
        // Call roundpictureservice to add the picture to the repository
        await _roundPicturesRepository.CreateAsync(roundPicture);
    }

    public async Task<PictureSelectedDTO> VoteForSelectedPicture(int roundPictureId, int matchId, int roundIndex, int userId)
    {
        bool isRoundInVotingState = await _roundsRepo
            .AnyAsync(r =>
                r.MatchId == matchId &&
                r.RoundIndex == roundIndex &&
                r.RoundState == Round.RoundStates.Voting);

        if (!isRoundInVotingState)
        {
            throw new RoundNotInVotingStateException();
        }

        RoundVote roundVote = new()
        {
            RoundPictureId = roundPictureId,
            UserId = userId,
        };

        await _roundVotesRepository.CreateAsync(roundVote);

        PictureSelectedDTO pictureSelectedDto = await _roundPicturesRepository.GetAsync(
            filter: rp => rp.Id == roundPictureId,
            map: rp => _mapper.Map<PictureSelectedDTO>(rp)) ??
            throw new NotFoundException("Invalid round picture id");

        return pictureSelectedDto;
    }

    private int CalculateRoundWinner(Round r)
    {
        RoundPicture? winnerPicture = null;
        int maxVotes = 0;

        foreach (RoundPicture rp in r.RoundPictures)
        {
            if(maxVotes < rp.RoundVotes.Count)
            {
                maxVotes = rp.RoundVotes.Count;
                winnerPicture = rp;
            }
        }

        if (winnerPicture == null)
            throw new NotFoundException();

        return winnerPicture.UserId!.Value;

    }
}
