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





    public PicturesService(IPicturesRepository picturesRepo, IMapper mapper, ILogger<PicturesService> logger)
    {
        _picturesRepo = picturesRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PictureDTO> UploadPicture(IFormFile pictureFile, int userId)
    {
        var picturePath = Guid.NewGuid().ToString() + ".jpg";

        var picture = new Picture()
        {
            UserId = userId,
            PicturePath = picturePath,
            PictureFile = pictureFile
        };
        _logger.LogDebug($"Picture path {picturePath} was uploaded by user with id {userId}");
        var createdPicture = await _picturesRepo.CreateAsync(picture);

        var pictureDto = _mapper.Map<PictureDTO>(createdPicture);

        _logger.LogInformation($"Picture with id {pictureDto.Id} and path {picturePath} was uploaded by user with id {userId}");

        return pictureDto;
    }
}
