using PhotoShowdownBackend.Extentions;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhotoShowdownBackend.WebSockets;

public class WebSocketRoomManager
{
    private readonly ConcurrentDictionary<int, WebSocketRoom> ChatRooms = new();
    public void AddSocket(int userId, int matchId, WebSocket socket)
    {
        WebSocketRoom room = GetOrCreateRoom(matchId);
        room.ConnectedUsers.TryAdd(userId, socket);
    }
    public void RemoveSocket(int userId, int matchId)
    {
        WebSocketRoom room = GetOrCreateRoom(matchId);
        room.ConnectedUsers.TryRemove(userId, out _);
    }
    public async Task SendMessage(int sendingUserId, int matchId, string message)
    {
        if (ChatRooms.TryGetValue(matchId, out var room))
        {
            foreach (var (userId, userSocket) in room.ConnectedUsers)
            {
                if (userId != sendingUserId)
                    await userSocket.SendMessageAsync(message);
            }
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
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                RemoveSocket(userId, matchId);
            }
        }
    }
    private WebSocketRoom GetOrCreateRoom(int matchId)
    {
        return ChatRooms.GetOrAdd(matchId, id => new WebSocketRoom(id));
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
