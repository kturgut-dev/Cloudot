namespace Cloudot.Infrastructure.Messaging.Email;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message);
}
