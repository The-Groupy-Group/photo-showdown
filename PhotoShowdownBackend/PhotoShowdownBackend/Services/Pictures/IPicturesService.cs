using PhotoShowdownBackend.Dtos.PicturesDto;

namespace PhotoShowdownBackend.Services.Pictures;

public interface IPicturesService
{
    Task<PictureDTO> UploadPicture(IFormFile pictureFile, int userId);
    Task<List<PictureDTO>> GetUserPicture(int userId);
    Task<PictureDTO> GetPicture(int pictureId);
    Task DeletePicture(int pictureId, int userId, bool isAdmin);
    Task<bool> DoesPictureBelongToUser(int pictureId, int userId);
}
