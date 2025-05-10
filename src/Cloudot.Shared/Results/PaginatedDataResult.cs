namespace Cloudot.Shared.Results;

public class PaginatedDataResult<T> : DataResult<List<T>>
{
    public int TotalCount { get; set; }
}