using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using PhotoShowdownBackend.Dtos.PicturesDto;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Session;


namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class PicturesController : ControllerBase
{
    private readonly IPicturesService _picturesService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<PicturesController> _logger;


    public PicturesController(IPicturesService picturesService, ISessionService sessionService, ILogger<PicturesController> logger)
    {
        _picturesService = picturesService;
        _sessionService = sessionService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a picture. Pictures can be retrived by accessing the path /pictures/{picturePath}
    /// </summary>
    /// <param name="pictureFile">File containing the picture</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<PictureDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadPicture(IFormFile pictureFile)
    {
        var response = new APIResponse<PictureDTO>();
        try
        {
            var currentUserId = _sessionService.GetCurrentUserId();
            var picture = await _picturesService.UploadPicture(pictureFile, currentUserId);

            // Append base path to picture path
            picture.PicturePath = GetPictureBaseBath() + picture.PicturePath;

            response.Data = picture;
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UploadPicture)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
        }
    }

    /// <summary>
    /// Returns all pictures uploaded by the current user
    /// </summary>
    /// <returns>A Array of all pictures uploaded by the current user</returns>
    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<IEnumerable<PictureDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyPictures()
    {
        var response = new APIResponse<IEnumerable<PictureDTO>>();
        try
        {
            var currentUserId = _sessionService.GetCurrentUserId();
            var pictures = await _picturesService.GetUserPicture( currentUserId);

            // Append base path to picture path
            var basePath = GetPictureBaseBath();
            foreach(var pic in pictures)
            {
                pic.PicturePath = basePath + pic.PicturePath;
            }

            response.Data = pictures;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UploadPicture)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
        }
    }
    [HttpPost("{id:int}")]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePicture(int id)
    {
        var response = new APIResponse();
        return Ok(response.ToErrorResponse("This function is not yet implemented"));
    }

    [NonAction]
    private string GetPictureBaseBath()
    {
        return $"{Request.Scheme}://{Request.Host}/pictures/";
    }
}
