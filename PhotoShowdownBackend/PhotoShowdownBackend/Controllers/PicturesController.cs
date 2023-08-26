using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<PictureDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadPicture(IFormFile pictureFile)
    {
        var response = new APIResponse<PictureDTO>();
        try
        {
            var currentUserId = _sessionService.GetCurrentUserId();
            var picture = await _picturesService.UploadPicture(pictureFile, currentUserId);

            response.Data = picture;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UploadPicture)} Error");
            return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.ToServerError());
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<IEnumerable<PictureDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyPictures()
    {
        var response = new APIResponse<List<PictureDTO>>();

        List<PictureDTO> pictures = new() {
            new()
            {
                Id = 1,
                PicturePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQak2UlOk7R-S2LGp_5kRqUFhnMXGjW49FJsJk2_LjXCQe1rFap1sRYgrLcQr8_d45-0oE&usqp=CAU"
            },
            new()
            {
                Id = 2,
                PicturePath ="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ85_JuOTzO3sdaFjvyRh2qUJ6sLJaUO0wO8mTeFJW0vMpPaLU7qEYWjW19FIHeco_Bqes&usqp=CAU"
            },
            new()
            {
                Id = 3,
                PicturePath ="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS5jfpIDgyFTP5lINIuuSIN4jNDDTt0dawTKz4EatXqGJ6--ZvAblZ-83vKm4uwa_HrQCk&usqp=CAU"
            },
            new()
            {
                Id = 4,
                PicturePath ="https://i.pinimg.com/1200x/72/19/1c/72191ce7873172bb0082e390ace5beef.jpg"
            },
            new()
            {
                Id = 5,
                PicturePath ="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRe0f31Vn6kriHg_-SLX4PAnIqegB5OtaeTD_CNQLxiU2kHqFmdKuqXhZjdtvYUOUz12dU&usqp=CAU"
            }
        };

        response.Data = pictures;

        return Ok(response.ToErrorResponse("This function is not yet implemented"));
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
}
