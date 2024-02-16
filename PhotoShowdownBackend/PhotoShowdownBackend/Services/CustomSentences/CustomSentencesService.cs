using AutoMapper;
using PhotoShowdownBackend.Repositories.CustomSentences;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Services.MatchConnections;

namespace PhotoShowdownBackend.Services.CustomSentences
{
    public class CustomSentencesService : ICustomSentencesService
    {
        private readonly ICustomSentencesRepository _customSentencesRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomSentencesService> _logger;

        public CustomSentencesService(ICustomSentencesRepository customSentencesRepo, IMapper mapper, ILogger<CustomSentencesService> logger)
        {
            _customSentencesRepo = customSentencesRepo;
            _mapper = mapper;
            _logger = logger;
        }
    }
}
