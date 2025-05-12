using System.Security.Claims;

namespace Cloudot.Infrastructure.Auth.Jwt;

public interface ITokenHelper
{
    /// <summary>
    /// Generates both access and refresh tokens for a user
    /// </summary>
    Task<TokenModel> GenerateTokensAsync(Dictionary<string,object> userInfo);
        
    /// <summary>
    /// Validates an access token and returns the user info
    /// </summary>
    Task<bool> ValidateAccessTokenAsync(string accessToken);
        
    /// <summary>
    /// Validates a refresh token and generates new tokens if valid
    /// </summary>
    Task<TokenModel?> RefreshTokensAsync(string refreshToken);
}