namespace Cloudot.Core.Utilities.Security.Tokens;

public class RefreshTokenInfo
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime Expiration { get; set; }
}