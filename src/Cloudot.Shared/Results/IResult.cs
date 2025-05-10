namespace Cloudot.Shared.Results;

public interface IResult
{
    bool IsSuccess { get; }
    string? Message { get; }
    int? StatusCode { get; set; }
    // List<MessageItem>? ValidationMessages { get; set; }
}
