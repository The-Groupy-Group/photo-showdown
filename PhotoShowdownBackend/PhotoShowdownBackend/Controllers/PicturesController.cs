﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PhotoShowdownBackend.Attributes;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Pictures;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Services.Session;


namespace PhotoShowdownBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
[HandleException]
[EnableRateLimiting("token")]
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

        if (!pictureFile.ContentType.Contains("image"))
            return BadRequest(response.ErrorResponse("Uploaded file is not a image"));

        var currentUserId = _sessionService.GetCurrentUserId();
        var picture = await _picturesService.UploadPicture(pictureFile, currentUserId);

        // Append base path to picture path
        picture.PicturePath = GetPictureBaseBath() + picture.PicturePath;

        response.Data = picture;

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Returns all pictures uploaded by the current user
    /// </summary>
    /// <returns>A Array of all pictures uploaded by the current user</returns>
    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<IEnumerable<PictureDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyPictures()
    {
        var response = new APIResponse<IEnumerable<PictureDTO>>();

        var currentUserId = _sessionService.GetCurrentUserId();
        var pictures = await _picturesService.GetUserPicture(currentUserId);

        // Append base path to picture path
        var basePath = GetPictureBaseBath();
        foreach (var pic in pictures)
        {
            pic.PicturePath = basePath + pic.PicturePath;
        }

        response.Data = pictures;
        return Ok(response);
    }

    /// <summary>
    /// Obviusly this deletes a picture
    /// </summary>
    /// <param name="pictureId"></param>
    /// <returns></returns>
    [HttpDelete("{pictureId:int}")]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePicture(int pictureId)
    {
        APIResponse response = new();
        try
        {
            var userId = _sessionService.GetCurrentUserId();
            bool isAdmin = _sessionService.IsCurrentUserInRole(Roles.Admin);

            await _picturesService.DeletePicture(pictureId, userId, isAdmin);

            return Ok(response);
        }
        catch (ResourceBelongsToDifferentUserException)
        {
            return NotFound(response.ErrorResponse(Messages.PictureNotFound));
        }
        catch (NotFoundException)
        {
            return NotFound(response.ErrorResponse(Messages.PictureNotFound));
        }
    }

    // ------------------ Helper Functions ------------------
    [NonAction]
    private string GetPictureBaseBath()
    {
        return $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{SystemSettings.PicturesFolderName}/";
    }
    internal static class Messages
    {
        public const string PictureNotFound = "Picture not found";
    }
}
