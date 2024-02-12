using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketRoom
{
    public int RoomId { get; }
    public ConcurrentDictionary<int, WebSocket> ConnectedUsers { get; }

    public WebSocketRoom(int roomId)
    {
        RoomId = roomId;
        ConnectedUsers = new ConcurrentDictionary<int, WebSocket>();
    }
}
