namespace Cloudot.Shared.Results;

public class Result : IResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public int StatusCode { get; set; } = 200;
    public List<MessageItem>? ValidationErrors { get; set; } = new List<MessageItem>();

    public Result()
    {
    }

    public Result(bool isSuccess, string? message = null) : base()
    {
        IsSuccess = isSuccess;
        Message = message;
    }


    public Result(bool isSuccess, string? message = null, int statusCode = 200) : base()
    {
        IsSuccess = isSuccess;
        Message = message;
        StatusCode = statusCode;
    }

    public static Result Success(string? message = null)
    {
        return new Result(true, message);
    }
    
    public static Result Success(string? message = null, int statusCode = 200)
    {
        return new Result(true, message, statusCode);
    }

    public static Result Fail(string? message = null)
    {
        return new Result(false, message);
    }

    public static Result Fail(string? message = null,int statusCode = 400)
    {
        return new Result(false, message, statusCode);
    }
}