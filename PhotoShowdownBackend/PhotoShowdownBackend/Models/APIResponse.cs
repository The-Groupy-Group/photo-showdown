namespace PhotoShowdownBackend.Models;

public class APIResponse<T> : APIResponse
{
    public T? Data { get; set; }

}


public class APIResponse
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
    public APIResponse ToErrorResponse(string message)
    {
        this.IsSuccess = false;
        this.Message = message;
        return this;
    }
    public static APIResponse ToServerError()
    {
        var response = new APIResponse
        {
            IsSuccess = false,
            Message = "Server error :("
        };
        return response;
    }
}