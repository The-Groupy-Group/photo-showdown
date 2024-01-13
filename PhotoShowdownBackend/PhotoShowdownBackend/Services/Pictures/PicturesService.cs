using AutoMapper;
using PhotoShowdownBackend.Dtos.PicturesDto;
using PhotoShowdownBackend.Exceptions;
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
        var picture = new Picture()
        {
            UserId = userId,
            PictureFile = pictureFile
        };

        var createdPicture = await _picturesRepo.CreateAsync(picture);

        var pictureDto = _mapper.Map<PictureDTO>(createdPicture);

        _logger.LogInformation($"Picture with id {pictureDto.Id} and path {pictureDto.PicturePath} was uploaded by user with id {userId}");

        return pictureDto;
    }

    public async Task<List<PictureDTO>> GetUserPicture(int userId)
    {
        var pictures = await _picturesRepo.GetAllAsync(p => p.UserId == userId, tracked:false);
        return pictures.Select(p => _mapper.Map<PictureDTO>(p)).ToList();
    }

    public async Task<PictureDTO> GetPicture(int pictureId)
    {
        var picture = await _picturesRepo.GetAsync(p=>p.Id == pictureId);
        return _mapper.Map<PictureDTO>(picture);
    }

    public async Task DeletePicture(int pictureId, int userId, bool isAdmin)
    {
        if (!isAdmin && !await DoesPictureBelongToUser(pictureId, userId)) 
            throw new UnauthorizedException("Picture does not belong to user");

        var picture = await _picturesRepo.GetAsync(p => p.Id == pictureId);

        if (picture == null) 
            throw new NotFoundException("Picture not found");

        await _picturesRepo.DeleteAsync(picture);
    }

    public async Task<bool> DoesPictureBelongToUser(int pictureId, int userId)
    {
        var picture = await _picturesRepo.GetAsync(p => p.Id == pictureId);

        if (picture == null) 
            throw new NotFoundException("Picture not found");

        return picture.UserId == userId;
    }
}
