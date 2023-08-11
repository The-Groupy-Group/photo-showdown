namespace PhotoShowdownBackend.Models;

public class APIResponse<T> : EmptyAPIResponse
{
    public T? Data { get; set; }

}


public class EmptyAPIResponse
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
    public EmptyAPIResponse ToErrorResponse(string message)
    {
        this.IsSuccess = false;
        this.Message = message;
        return this;
    }
    public static EmptyAPIResponse ToServerError()
    {
        var response = new EmptyAPIResponse
        {
            IsSuccess = false,
            Message = "Server error :("
        };
        return response;
    }
}