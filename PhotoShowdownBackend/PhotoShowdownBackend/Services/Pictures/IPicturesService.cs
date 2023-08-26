using PhotoShowdownBackend.Dtos.PicturesDto;

namespace PhotoShowdownBackend.Services.Pictures;

public interface IPicturesService
{
    Task<PictureDTO> UploadPicture(IFormFile pictureFile, int userId);
}
