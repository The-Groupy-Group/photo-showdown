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
            "The best sentence to start talking to a girl ___.",
            "I went to the grocery store and asked for my  favorite food which is___.",
            "On Christmas my aunt brought me the gift I wanted the most.",
            "I hate washing dishes, it always ends with___.",
            "At my birthday party I invited all my friends except___, whom I don't like.",
            "When she asks me to___, I always tell  her___.",
            "The best way to be in two places at the same time___.",
            "With great power comes great responsibility.",
            "If I had one wish I would ask___.",
            "The one thing i will never do___.",
            "Most of the times i help a friend, i expect for return___.",
            "My favorite game is___, I always win.",
            "My favorite sentence."
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

        public async Task SetCustomSentences(List<string> sentenes, int matchId)
        {
            await _customSentencesRepo.CreateRangeAsync(
                sentenes
                .Select(s => new CustomSentence
                {
                    Sentence = s,
                    MatchId = matchId
                })
                .ToArray());
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
