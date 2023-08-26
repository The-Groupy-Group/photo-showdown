using AutoMapper;
using PhotoShowdownBackend.Dtos.PicturesDto;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Pictures;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Services.Pictures;

public class PicturesService : IPicturesService
{
    private readonly IPicturesRepository _picturesRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PicturesService> _logger;
    private readonly IWebHostEnvironment _environment;


    public PicturesService(IPicturesRepository picturesRepo, IMapper mapper, ILogger<PicturesService> logger, IWebHostEnvironment environment)
    {
        _picturesRepo = picturesRepo;
        _mapper = mapper;
        _logger = logger;
        _environment = environment;
    }

    public async Task<PictureDTO> UploadPicture(IFormFile pictureFile, int userId)
    {
        var randomUniqueName = Guid.NewGuid().ToString();
        var path = GetPicturePath(randomUniqueName);

        var picture = new Picture()
        {
            UserId = userId,
            PicturePath = path,
            PictureFile = pictureFile
        };

        var createdPicture = await _picturesRepo.CreateAsync(picture);

        var pictureDto = _mapper.Map<PictureDTO>(createdPicture);
        return pictureDto;
    }

    private string GetPicturePath(string pictureName)
    {
        return Path.Combine(_environment.ContentRootPath, "wwwroot", "pictures", pictureName + ".jpg");
    }
}
