using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Utils;
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
        string res = JsonSerializer.Serialize<object>(this, SystemSettings.JsonSerializerOptions);
        return res;
    }
    public enum MessageType
    {
        PlayerJoined,
        PlayerLeft,
        NewOwner,
        MatchStarted,
        RoundStateChange,
        UserLockedIn,
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