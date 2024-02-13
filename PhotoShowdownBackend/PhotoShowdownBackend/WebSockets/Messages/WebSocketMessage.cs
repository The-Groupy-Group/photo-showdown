using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhotoShowdownBackend.WebSockets.Messages;

public abstract class WebSocketMessage
{
    public MessageType Type { get; set; }
    public WebSocketMessage(MessageType type)
    {
        Type = type;
    }
    override public string ToString()
    {
        var res = JsonSerializer.Serialize((object)this, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true // Optional: Set to true if you want an indented JSON string
        });
        return res;
    }
    public enum MessageType
    {
        PlayerJoined,
        PlayerLeft,
    }
}
