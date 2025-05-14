namespace Cloudot.Infrastructure.Auth.Jwt;

/// <summary>
/// JWT token üretildiğinde dönecek olan access ve refresh token bilgilerini içerir.
/// </summary>
public class JwtTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
}
