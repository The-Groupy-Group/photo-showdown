using AutoMapper;
using PhotoShowdownBackend.Repositories.Pictures;
using PhotoShowdownBackend.Repositories.Rounds;
using PhotoShowdownBackend.Services.Pictures;

namespace PhotoShowdownBackend.Services.Rounds;

public class RoundsService: IRoundsService
{
    private readonly IRoundsRepository _roundsRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<RoundsService> _logger;

    public RoundsService(IRoundsRepository roundsRepo,IMapper mapper,ILogger<RoundsService> logger)
    {
        _roundsRepo = roundsRepo;
        _mapper = mapper;
        _logger = logger;
    }

}
