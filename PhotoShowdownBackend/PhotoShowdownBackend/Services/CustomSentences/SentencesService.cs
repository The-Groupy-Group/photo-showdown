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
        private static ConcurrentDictionary<int,List<string>> _defaultSentences = new();

        public SentencesService(ICustomSentencesRepository customSentencesRepo, IMapper mapper, ILogger<SentencesService> logger)
        {
            _customSentencesRepo = customSentencesRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> FetchSentence(int matchId)
        {
    
            var customSentence = await _customSentencesRepo.GetAsync(cs => cs.MatchId == matchId);

            if (customSentence == null)
            {
                List<string> SentencesListForMatch = _defaultSentences.GetOrAdd(matchId, matchId => InitDefaultSentencesForMatch());
                var defaultSentence = SentencesListForMatch.FirstOrDefault();
                SentencesListForMatch.Remove(defaultSentence);
                return defaultSentence ?? ""; //todo - throw error if there are no more sentences, and end match bamyadit.
            }
            else
            {
                await _customSentencesRepo.DeleteAsync(customSentence);
                return customSentence.Sentence;
            }
            
        }

        private List<string> InitDefaultSentencesForMatch()
        {
            List<string> sentences = new()
            {
                "",
            };
            Random rnd = new();
            return sentences.OrderBy(s => rnd.Next(int.MaxValue)).ToList();
        }

    }
}
