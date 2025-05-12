namespace Cloudot.Infrastructure.Auth.Jwt;

public class TokenSettings : ITokenSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15; // Default: 15 minutes
    public int RefreshTokenExpirationDays { get; set; } = 7;    // Default: 7 days
}