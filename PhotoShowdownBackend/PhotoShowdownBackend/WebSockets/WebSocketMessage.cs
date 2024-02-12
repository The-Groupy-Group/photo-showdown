using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhotoShowdownBackend.WebSockets;

public class WebSocketMessage
{
    public MessageType Type { get; set; }
    public object Data { get; set; }
    public WebSocketMessage(object date, MessageType type)
    {
        Data = date;
        Type = type;
    }
    override public string ToString()
    {
        var res = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true // Optional: Set to true if you want an indented JSON string
        });
        return res;
    }
    public enum MessageType
    {
        playerJoined,
    }
}
