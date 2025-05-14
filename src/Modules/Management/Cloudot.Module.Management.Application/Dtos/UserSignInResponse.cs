namespace Cloudot.Module.Management.Application.Dtos;

public class UserSignInResponse
{
    public string Email { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime Expiration { get; set; }
}