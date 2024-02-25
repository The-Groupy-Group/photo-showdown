using PhotoShowdownBackend.Dtos;
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
    private readonly ConcurrentDictionary<int, WebSocketRoom> ChatRooms = new();
    private readonly ILogger<WebSocketRoomManager> _logger;
    // Constructor
    public WebSocketRoomManager(ILogger<WebSocketRoomManager> logger)
    {
        _logger = logger;
    }
    // Public methods
    public void AddSocket(int userId, int matchId, WebSocket socket)
    {
        WebSocketRoom room = GetOrCreateRoom(matchId);
        room.ConnectedUsers.TryAdd(userId, socket);
    }
    public async Task SendMessageToRoom(int? sendingUserId, int matchId, WebSocketMessage message)
    {
        _logger.LogInformation("Sending web socket message by user {userId} to match {matchId}: {message}", sendingUserId, matchId, message);
        if (ChatRooms.TryGetValue(matchId, out var room))
        {
            foreach (var (userId, userSocket) in room.ConnectedUsers)
            {
                if (userId != sendingUserId && userSocket.State == WebSocketState.Open)
                    await userSocket.SendMessageAsync(message.ToString());
            }
        }
        else
        {
            _logger.LogWarning("Match {matchId} not found", matchId);
        }
    }
    public async Task HandleWebSocket(WebSocket webSocket, int userId, int matchId)
    {
        // Handle the web socket
        while (webSocket.State == WebSocketState.Open)
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);

                await webSocket.SendMessageAsync(JsonSerializer.Serialize("Echo: " + message));
            }
            if (result.MessageType == WebSocketMessageType.Close)
            {
                RemoveSocket(userId, matchId);
            }
        }
    }
    public async Task CloseConnection(int userId, int matchId)
    {
        if (ChatRooms.TryGetValue(matchId, out var room))
        {
            if (room.ConnectedUsers.TryGetValue(userId, out var socket))
            {
                await socket.CloseConnection();
                RemoveSocket(userId, matchId);
            }
        }
    }
    public async Task CloseRoom(int matchId)
    {
        var room = GetOrCreateRoom(matchId);
        foreach (var (_, socket) in room.ConnectedUsers)
        {
            await socket.CloseConnection();
        }
        ChatRooms.TryRemove(matchId, out _);
    }
    // Private methods
    private WebSocketRoom GetOrCreateRoom(int matchId)
    {
        return ChatRooms.GetOrAdd(matchId, id => new WebSocketRoom(id));
    }
    private void RemoveSocket(int userId, int matchId)
    {
        WebSocketRoom room = GetOrCreateRoom(matchId);
        room.ConnectedUsers.TryRemove(userId, out _);
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
