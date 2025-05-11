namespace Cloudot.Shared.Results;

public class DataResult<T> : Result, IDataResult<T>
{
    public T? Data { get; set; }

    public DataResult() : base()
    {
        
    }
    
    public DataResult(T? data, bool isSuccess, string? message = null)
        : base(isSuccess, message)
    {
        Data = data;
    }

    public static DataResult<T> Success(T data, string? message = null)
    {
        return new DataResult<T>(data, true, message);
    }

    public static DataResult<T> Fail(string? message = null)
    {
        return new DataResult<T>(default, false, message);
    }
}
