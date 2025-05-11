namespace Cloudot.Shared.Results;

public class Result : IResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public int? StatusCode { get; set; }
    // public List<MessageItem>? Messages { get; set; }

    public Result()
    {
        
    }
    
    public Result(bool isSuccess, string? message = null) : base()
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success(string? message = null)
    {
        return new Result(true, message);
    }

    public static Result Fail(string? message = null)
    {
        return new Result(false, message);
    }
}
