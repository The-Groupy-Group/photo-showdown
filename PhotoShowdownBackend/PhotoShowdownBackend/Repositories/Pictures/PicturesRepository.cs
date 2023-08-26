using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Pictures;

public class PicturesRepository : Repository<Picture>, IPicturesRepository
{
    public PicturesRepository(PhotoShowdownDbContext db) : base(db)
    {
    }
    override public Task<Picture> CreateAsync(Picture picture)
    {
        if (picture.PictureFile == null)
        {
            throw new ArgumentNullException(nameof(picture.PictureFile));
        }
        // Save picture file to disk
        SavePictureFile(picture.PictureFile, picture.PicturePath);
        // Save picture to database
        return base.CreateAsync(picture);
    }

    /// <summary>
    /// Helper method to save picture file to disk
    /// </summary>
    /// <param name="pictureFile"></param>
    /// <param name="path"></param>
    private async void SavePictureFile(IFormFile pictureFile, string path)
    {
        using (var fileStream = new FileStream(path, FileMode.Create))
        {
            await pictureFile.CopyToAsync(fileStream);
        }
    }
}
