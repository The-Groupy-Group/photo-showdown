using AutoMapper;
using PhotoShowdownBackend.Repositories.Pictures;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Services.Pictures;

public class PicturesService: IPicturesService
{
    private readonly IPicturesRepository _picturesRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PicturesService> _logger;


    public PicturesService(IPicturesRepository picturesRepo, IConfiguration configuration, IMapper mapper, ILogger<PicturesService> logger)
    {
        _picturesRepo = picturesRepo;
        _mapper = mapper;
        _logger = logger;
    }
}
