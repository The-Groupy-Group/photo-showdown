using System.Net.WebSockets;

namespace PhotoShowdownBackend.Extentions;

public static class WebSocketExtentions
{
    public static async Task SendMessageAsync(this WebSocket socket, string message)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<byte>(bytes);

        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, default);
    }
}
