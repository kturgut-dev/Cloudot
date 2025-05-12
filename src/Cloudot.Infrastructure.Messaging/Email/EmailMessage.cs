using System.Net.Mail;

namespace Cloudot.Infrastructure.Messaging.Email;

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public List<Attachment>? Attachments { get; set; }
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
}