using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cloudot.Infrastructure.Messaging.Email;

public class SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger) : IEmailSender
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

        using MailMessage mailMessage = new()
        {
            From = new MailAddress(from, displayName, Encoding.UTF8),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        mailMessage.To.Add(message.To);
        if (!string.IsNullOrWhiteSpace(message.Cc))
            mailMessage.CC.Add(message.Cc);

        if (!string.IsNullOrWhiteSpace(message.Bcc))
            mailMessage.Bcc.Add(message.Bcc);

        if (message.Attachments is not null)
        {
            foreach (Attachment attachment in message.Attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }
        }

        using SmtpClient client = new(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(username, password)
        };
        
        await client.SendMailAsync(mailMessage);
        _logger.LogInformation("E-posta gönderildi: {To}", message.To);
    }
}
