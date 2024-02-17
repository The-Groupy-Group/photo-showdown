using AutoMapper;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.CustomSentences;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Services.MatchConnections;
using System.Collections.Concurrent;

namespace PhotoShowdownBackend.Services.CustomSentences
{
    public class SentencesService : ISentencesService
    {
        private readonly ICustomSentencesRepository _customSentencesRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<SentencesService> _logger;
        private static readonly ConcurrentDictionary<int, ConcurrentBag<string>> _defaultSentencesRepo = new();
        private static readonly List<string> _defaultSentences = new()
        {
            "Diat from tomorrow",
        };

        public SentencesService(ICustomSentencesRepository customSentencesRepo, IMapper mapper, ILogger<SentencesService> logger)
        {
            _customSentencesRepo = customSentencesRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string?> FetchSentence(int matchId)
        {

            var customSentence = await _customSentencesRepo.GetAsync(cs => cs.MatchId == matchId);

            if (customSentence == null)
            {
                ConcurrentBag<string> SentencesListForMatch = _defaultSentencesRepo.GetOrAdd(matchId, matchId => InitDefaultSentencesForMatch());
                SentencesListForMatch.TryTake(out string? defaultSentence);
                return defaultSentence;
            }
            else
            {
                await _customSentencesRepo.DeleteAsync(customSentence);
                return customSentence.Sentence;
            }
        }

        private static ConcurrentBag<string> InitDefaultSentencesForMatch()
        {
            Random rnd = new();
            ConcurrentBag<string> sentences = new();
            _defaultSentences.OrderBy(x => rnd.Next()).ToList().ForEach(sentences.Add);
            return sentences;
        }
    }
}
