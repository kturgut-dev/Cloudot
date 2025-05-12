namespace Cloudot.Infrastructure.Auth.Jwt;

public interface ITokenSettings
{
    string Issuer { get; }
    string Audience { get; }
    string SecretKey { get; }
    int AccessTokenExpirationMinutes { get; }
    int RefreshTokenExpirationDays { get; }
}