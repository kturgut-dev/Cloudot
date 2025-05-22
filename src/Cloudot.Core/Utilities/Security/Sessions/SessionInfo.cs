namespace Cloudot.Core.Utilities.Security.Sessions;

public class SessionInfo
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
}