namespace PhotoShowdownBackend.Models;

public class APIResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
    public APIResponse<T> ToErrorResponse(string message)
    {
        this.IsSuccess = false;
        this.Message = message;
        return this;
    }
    public static APIResponse<T> ToServerError()
    {
        var response = new APIResponse<T>
        {
            IsSuccess = false,
            Message = "Server error :("
        };
        return response;
    }
}
