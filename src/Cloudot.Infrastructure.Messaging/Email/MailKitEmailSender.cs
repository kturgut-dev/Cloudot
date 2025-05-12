using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Cloudot.Infrastructure.Messaging.Email;

public class MailKitEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<SmtpEmailSender> _logger = logger;

    public async Task SendAsync(EmailMessage message)
    {
        string host = _configuration["Email:Smtp:Host"]!;
        int port = int.Parse(_configuration["Email:Smtp:Port"]!);
        string username = _configuration["Email:Smtp:Username"]!;
        string password = _configuration["Email:Smtp:Password"]!;
        string from = _configuration["Email:Smtp:From"]!;
        string displayName = _configuration["Email:Smtp:DisplayName"] ?? "";

        MimeMessage email = new();
        email.From.Add(new MailboxAddress(displayName, from));
        email.To.Add(MailboxAddress.Parse(message.To));

        if (!string.IsNullOrWhiteSpace(message.Cc))
            email.Cc.Add(MailboxAddress.Parse(message.Cc));

        if (!string.IsNullOrWhiteSpace(message.Bcc))
            email.Bcc.Add(MailboxAddress.Parse(message.Bcc));

        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.IsHtml ? message.Body : null,
            TextBody = message.IsHtml ? null : message.Body
        };

        if (message.Attachments is not null)
        {
            foreach (var attachment in message.Attachments)
            {
                using MemoryStream ms = new();
                await attachment.ContentStream.CopyToAsync(ms);
                bodyBuilder.Attachments.Add(attachment.Name, ms.ToArray(),
                    ContentType.Parse(attachment.ContentType.MediaType));
            }
        }

        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
        await smtp.AuthenticateAsync(username, password);
        string asd = await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        _logger.LogInformation("E-posta gönderildi: {To}", message.To);
    }
}