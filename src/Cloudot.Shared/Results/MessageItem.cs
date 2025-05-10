using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Results;

public class MessageItem
{
    public string Code { get; set; } = null!;
    public string Message { get; set; } = null!;
    public MessageLevel Level { get; set; } = MessageLevel.Error;
}
