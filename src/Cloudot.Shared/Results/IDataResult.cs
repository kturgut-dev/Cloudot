namespace Cloudot.Shared.Results;

public interface IDataResult<T> : IResult
{
    T? Data { get; set; }
}
