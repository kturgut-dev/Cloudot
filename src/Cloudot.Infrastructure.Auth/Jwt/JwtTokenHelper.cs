using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cloudot.Infrastructure.Auth.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cloudot.Infrastructure.Auth.Jwt;

/// <summary>
/// JWT token üretimi ve doğrulama işlemleri için yardımcı sınıf.
/// </summary>
public class JwtTokenHelper(IConfiguration configuration) : IJwtTokenHelper
{
    private readonly string _issuer = configuration["Jwt:Issuer"]!;
    private readonly string _audience = configuration["Jwt:Audience"]!;
    private readonly string _key = configuration["Jwt:Key"]!;
    private readonly int _accessTokenMinutes = int.Parse(configuration["Jwt:AccessTokenMinutes"] ?? "15");
    private readonly int _refreshTokenDays = int.Parse(configuration["Jwt:RefreshTokenDays"] ?? "7");

    /// <inheritdoc />
    public JwtTokenResponse CreateToken(string userId, string email, IEnumerable<Claim>? additionalClaims = null)
    {
        DateTime now = DateTime.UtcNow;
        DateTime accessExpires = now.AddMinutes(_accessTokenMinutes);
        DateTime refreshExpires = now.AddDays(_refreshTokenDays);

        List<Claim> claims = new()
        {
            new Claim(AuthClaimTypes.UserId, userId),
            // new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(AuthClaimTypes.Email, email),
            // new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (additionalClaims is not null)
            claims.AddRange(additionalClaims);

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_key));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken accessToken = new(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: accessExpires,
            signingCredentials: creds
        );

        string accessTokenStr = new JwtSecurityTokenHandler().WriteToken(accessToken);

        // Refresh token basitçe oluşturulur (gerçekte veritabanında saklanır)
        string refreshToken = Guid.NewGuid().ToString("N");

        return new JwtTokenResponse
        {
            AccessToken = accessTokenStr,
            AccessTokenExpiresAt = accessExpires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpires
        };
    }

    /// <inheritdoc />
    public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
    {
        TokenValidationParameters parameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        };

        JwtSecurityTokenHandler handler = new();
        try
        {
            ClaimsPrincipal principal = handler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}