namespace PhotoShowdownBackend.Utils;

public class APIResponse<T> : APIResponse
{
    public T? Data { get; set; }
}


public class APIResponse
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
    public APIResponse ErrorResponse(string message)
    {
        IsSuccess = false;
        Message = message;
        return this;
    }
    public static APIResponse ServerError => new()
    {
        IsSuccess = false,
        Message = "Server error :("
    };
}