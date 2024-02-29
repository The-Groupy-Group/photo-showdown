using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.WebSockets;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShowdownBackend.Middlewares;

// DEPRECATED SWAPPED FOR WebSocketsController
public class WebSocketHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public WebSocketHandlerMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context, IMatchesService matchesService, WebSocketRoomManager webSocketManager)
    {
        // If the request is not a websocket request, continue to the next middleware
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next.Invoke(context);
            return;
        }

        // Extract the JWT token from the query string
        string jwtSecret = _configuration["AppSettings:Token"]!;
        string? jwtFromQueryString = context.Request.Query["jwt"];

        if (string.IsNullOrEmpty(jwtFromQueryString))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        // Validate the JWT token
        if (!ValidateJwtToken(jwtFromQueryString, jwtSecret, out int userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        // Get the match by the user id
        var match = await matchesService.GetMatchByUserId(userId);

        if (match == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        // Accept the websocket request and add it to the web socket manager
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await webSocketManager.AddWebSocket(userId, match.Id, socket);
    }

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