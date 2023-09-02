using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Pictures;

public class PicturesRepository : Repository<Picture>, IPicturesRepository
{
    private readonly IWebHostEnvironment _environment;

    private readonly string _picturesFolderPath;
    private readonly string _picturesFolderName = "pictures";

    public PicturesRepository(PhotoShowdownDbContext db, IWebHostEnvironment environment) : base(db)
    {
        _environment = environment;
        _picturesFolderPath = Path.Combine(_environment.WebRootPath, _picturesFolderName);
    }

    public async override Task<Picture> CreateAsync(Picture picture)
    {
        if (picture.PictureFile == null)
        {
            throw new ArgumentNullException(nameof(picture.PictureFile));
        }

        // Save picture file to disk
        var pictureAbsolutePath = GetAbsolutePicturePath(picture.PicturePath);

        await SavePictureFileAsync(picture.PictureFile, pictureAbsolutePath);

        // Save picture to database
        return await base.CreateAsync(picture);
    }

    public override Task<Picture> DeleteAsync(Picture entity)
    {
        var pictureAbsolutePath = GetAbsolutePicturePath(entity.PicturePath);

        DeletePictureFile(pictureAbsolutePath);

        return base.DeleteAsync(entity);
    }

    /// <summary>
    /// Helper method to save picture file to disk
    /// </summary>
    /// <param name="pictureFile"></param>
    /// <param name="absolutePath"></param>
    private static async Task SavePictureFileAsync(IFormFile pictureFile, string absolutePath)
    {
        using var fileStream = new FileStream(absolutePath, FileMode.Create);
        await pictureFile.CopyToAsync(fileStream);
    }

    private static void DeletePictureFile(string absolutePath)
    {
        try
        {
            if(File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
        catch { }
    }

    private string GetAbsolutePicturePath(string pictureName)
    {
        return Path.Combine(_picturesFolderPath, pictureName);
    }
}
