using System.Security.Claims;

namespace Cloudot.Infrastructure.Auth.Jwt;

/// <summary>
/// JWT token işlemlerini tanımlar (token oluşturma ve doğrulama).
/// </summary>
public interface IJwtTokenHelper
{
    /// <summary>
    /// Access ve refresh token üretir.
    /// </summary>
    /// <param name="userId">Kullanıcının Id değeri</param>
    /// <param name="email">Kullanıcının e-posta adresi</param>
    /// <param name="additionalClaims">Varsa ek claim listesi</param>
    JwtTokenResponse CreateToken(Guid userId, string email, IEnumerable<Claim>? additionalClaims = null);

    /// <summary>
    /// JWT token’ı doğrular ve geçerliyse ClaimsPrincipal olarak döner.
    /// </summary>
    /// <param name="token">JWT token değeri</param>
    /// <param name="validateLifetime">Süre kontrolü yapılacak mı</param>
    ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);
}