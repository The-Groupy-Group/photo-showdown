using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Extentions;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace PhotoShowdownBackend.WebSockets;

/// <summary>
/// Singletone class that manages web socket connections
/// </summary>
public class WebSocketRoomManager
{
    // Private fields
    private readonly ConcurrentDictionary<int, WebSocketRoom> _chatRooms = new();
    private readonly ILogger<WebSocketRoomManager> _logger;

    // Constructor
    public WebSocketRoomManager(ILogger<WebSocketRoomManager> logger)
    {
        _logger = logger;
    }

    // --------------- Public methods --------------- //

    /// <summary>
    /// Handles the web socket connection
    /// </summary>
    /// <param name="webSocket"></param>
    /// <param name="userId"></param>
    /// <param name="matchId"></param>
    /// <returns></returns>
    public async Task HandleWebSocket(WebSocket webSocket, int userId, int matchId)
    {
        // Handle the web socket
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    _logger.LogInformation("Received web socket message from user {userId} in match {matchId}", userId, matchId);

                    var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);

                    await webSocket.SendMessageAsync(JsonSerializer.Serialize("Echo: " + message));
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    _logger.LogWarning("Received binary web socket message from user {userId} in match {matchId}", userId, matchId);

                    var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);

                    await webSocket.SendMessageAsync(JsonSerializer.Serialize("Echo: " + message));
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation("Received web socket close message from user {userId} in match {matchId}", userId, matchId);

                    RemoveSocketFromRoom(userId, matchId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle web socket connection for user {userId}", userId);
            RemoveSocketFromRoom(userId, matchId);
        }
    }

    /// <summary>
    /// Adds a web socket to the room
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="matchId"></param>
    /// <param name="socket"></param>
    public void AddWebSocket(int userId, int matchId, WebSocket socket)
    {
        WebSocketRoom room = GetOrCreateRoom(matchId);

        if (!room.ConnectedUsers.TryAdd(userId, socket))
        {
            _logger.LogWarning("Failed to add web socket for user {userId} to match {matchId} because it already exists", userId, matchId);
        }

        _logger.LogInformation("Adding web socket for user {userId} to match {matchId}", userId, matchId);
    }

    /// <summary>
    /// Sends a message to the room
    /// </summary>
    /// <param name="sendingUserId"></param>
    /// <param name="matchId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageToRoom(int? sendingUserId, int matchId, WebSocketMessage message)
    {
        _logger.LogInformation("Sending web socket message by user {userId} to match {matchId}: {message}", sendingUserId, matchId, message);
        if (_chatRooms.TryGetValue(matchId, out var room))
        {
            foreach (var (userId, userSocket) in room.ConnectedUsers)
            {
                if (userId != sendingUserId && userSocket.State == WebSocketState.Open)
                    await userSocket.SendMessageAsync(message.ToString());
            }
        }
        else
        {
            _logger.LogWarning("Sending web socket message FAIL, Match {matchId} not found", matchId);
        }
    }

    /// <summary>
    /// Removes a web socket from the room and closes the connection
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="matchId"></param>
    /// <returns></returns>
    public async Task RemoveSocket(int userId, int matchId)
    {
        WebSocket? socket = RemoveSocketFromRoom(userId, matchId);
        if (socket != null)
        {
            await socket.CloseConnection();
        }
    }

    public async Task CloseRoom(int matchId)
    {
        var room = GetOrCreateRoom(matchId);
        foreach (var (_, socket) in room.ConnectedUsers)
        {
            await socket.CloseConnection();
        }
        _chatRooms.TryRemove(matchId, out _);
    }
    // --------------- Private methods --------------- //
    private WebSocketRoom GetOrCreateRoom(int matchId)
    {
        return _chatRooms.GetOrAdd(matchId, id => new WebSocketRoom(id));
    }
    private WebSocket? RemoveSocketFromRoom(int userId, int matchId)
    {
        if (!_chatRooms.TryGetValue(matchId, out WebSocketRoom? room) || room == null)
        {
            _logger.LogWarning("RemoveSocketFromRoom FAIL, Web socket room with id {matchId} not found", matchId);
            return null;
        }
        room.ConnectedUsers.TryRemove(userId, out WebSocket? socket);
        return socket!;
    }
    private class WebSocketRoom
    {
        public int RoomId { get; }
        public ConcurrentDictionary<int, WebSocket> ConnectedUsers { get; }

        public WebSocketRoom(int roomId)
        {
            RoomId = roomId;
            ConnectedUsers = new ConcurrentDictionary<int, WebSocket>();
        }
    }
}
