using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhotoShowdownBackend.Dtos;

/// <summary>
/// Base class for all web socket messages
/// Inherit this class and set the type of the message
/// </summary>
public abstract class WebSocketMessage
{
    public MessageType Type { get; set; }
    public WebSocketMessage(MessageType type)
    {
        Type = type;
    }
    override public string ToString()
    {
        var res = JsonSerializer.Serialize<object>(this, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
        return res;
    }
    public enum MessageType
    {
        PlayerJoined,
        PlayerLeft,
        NewOwner,
        MatchStarted,
        RoundStateChange,
        UserVotedToPicture,
    }
}

public abstract class WebSocketMessage<T> : WebSocketMessage
{
    public T Data { get; set; }
    protected WebSocketMessage(T data, MessageType type) : base(type)
    {
        Data = data;
    }
}