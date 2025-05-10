namespace Cloudot.Shared.Results;

public class DataResult<T> : Result, IDataResult<T>
{
    public T? Data { get; set; }
}
