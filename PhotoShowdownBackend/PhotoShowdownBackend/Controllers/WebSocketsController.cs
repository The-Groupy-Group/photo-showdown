﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PhotoShowdownBackend.Attributes;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Messages;
using PhotoShowdownBackend.Dtos.WebSocketMessages;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.WebSockets;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace PhotoShowdownBackend.Controllers;

/// <summary>
/// The controller of lies... I mean websockets :)
/// </summary>
[Route("api/ws")]
[ApiController]
[HandleException]
public class WebSocketsController : ControllerBase
{
    private readonly WebSocketRoomManager _webSocketRoomManager;
    private readonly IMatchesService _matchesService;
    private readonly string _jwtSecret;

    public WebSocketsController(
        WebSocketRoomManager webSocketRoomManager,
        IMatchesService matchesService,
        IConfiguration configuration)
    {
        _webSocketRoomManager = webSocketRoomManager;
        _matchesService = matchesService;
        _jwtSecret = configuration["AppSettings:Token"]!;
    }

    /// <summary>
    /// Adds a websocket to the web socket manager
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> AddWebSocket(string jwt)
    {
        var context = ControllerContext.HttpContext;
        // If the request is not a websocket request, continue to the next middleware
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return BadRequest();
        }
        if (string.IsNullOrEmpty(jwt))
        {
            return BadRequest();
        }
        // Validate the JWT token
        if (!ValidateJwtToken(jwt, _jwtSecret, out int userId))
        {
            return Unauthorized();
        }
        // Get the match by the user id
        var match = await _matchesService.GetMatchByUserId(userId);

        if (match == null)
        {
            return BadRequest();
        }

        // Accept the websocket request and add it to the web socket manager
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await _webSocketRoomManager.AddWebSocket(userId, match.Id, socket);
        return new EmptyResult();
    }

    // ----------------- Websocket messages documentation ----------------- //

    /// <summary>
    /// A message that is sent when a new owner is selected
    /// </summary>
    /// <returns> New owner message </returns>
    [HttpGet(nameof(WebSocketMessage.MessageType.NewOwner))]
    public ActionResult<NewOwnerWebSocketMessage> NewOwner()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// A message that is sent when a match is started
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(WebSocketMessage.MessageType.MatchStarted))]
    public ActionResult<MatchStartedWebSocketMessage> MatchStarted()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// A message that is sent when a player joins the match
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(WebSocketMessage.MessageType.PlayerJoined))]
    public ActionResult<PlayerJoinedWebSocketMessage> PlayerJoined()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// A message that is sent when a player leaves the match
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(WebSocketMessage.MessageType.PlayerLeft))]
    public ActionResult<PlayerLeftWebSocketMessage> PlayerLeft()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// A message that is sent when a player locks in
    /// </summary>
    /// <returns>The user id</returns>
    [HttpGet(nameof(WebSocketMessage.MessageType.UserLockedIn))]
    public ActionResult<UserLockedInWebSocketMessage> UserLockedIn()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// A message that is sent when the round state changes
    /// </summary>
    /// <returns>The intire round</returns>
    [HttpGet(nameof(WebSocketMessage.MessageType.RoundStateChange))]
    public ActionResult<RoundStateChangeWebSocketMessage> RoundStateChange()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // ----------------- Private functions ----------------- //
    private static bool ValidateJwtToken(string token, string secret, out int userId)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            userId = int.Parse(jwtToken.Claims
                .Where(c => c.Type == UserClaims.Id)
                .Select(c => c.Value)
                .First());

            return true;
        }
        catch
        {
            userId = -1;
            return false;
        }
    }
}
