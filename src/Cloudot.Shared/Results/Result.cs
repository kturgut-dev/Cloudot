namespace Cloudot.Shared.Results;

public class Result : IResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public int? StatusCode { get; set; }
    // public List<MessageItem>? Messages { get; set; }
}
