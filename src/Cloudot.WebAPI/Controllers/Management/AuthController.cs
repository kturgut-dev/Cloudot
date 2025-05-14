using System.Security.Claims;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudot.WebAPI.Controllers.Management;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService _authService) : MainController
{
    /// <summary>
    /// Kullanıcıya OTP gönderir
    /// </summary>
    [HttpPost("request-otp")]
    [ProducesResponseType(typeof(IDataResult<UserSignInResponse>), 200)]
    public async Task<IActionResult> RequestOtp([FromBody] UserSignInDto dto)
    {
        return ReturnResult(await _authService.RequestOtpAsync(dto));
    }

    /// <summary>
    /// OTP doğrulaması yapar ve token üretir
    /// </summary>
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(IDataResult<UserSignInResponse>), 200)]
    public async Task<IActionResult> VerifyOtp([FromBody] UserVerifyOtpDto dto)
    {
        return ReturnResult(await _authService.VerifyOtpAndSignInAsync(dto));
    }
    
    /// <summary>
    /// Yeni kullanıcı kaydı için OTP gönderir
    /// </summary>
    [HttpPost("signup")]
    [ProducesResponseType(typeof(Cloudot.Shared.Results.IResult), 200)]
    public async Task<IActionResult> SignUp([FromBody] UserSignUpDto dto)
    {
        return ReturnResult(await _authService.SignUpAsync(dto));
    }

    /// <summary>
    /// Kayıt OTP doğrulaması
    /// </summary>
    [HttpPost("signup/verify")]
    [ProducesResponseType(typeof(Cloudot.Shared.Results.IResult), 200)]
    public async Task<IActionResult> VerifySignUpOtp([FromBody] UserVerifyOtpDto dto)
    {
        return ReturnResult(await _authService.VerifySignUpOtpAsync(dto));
    }
    
    /// <summary>
    /// Refresh token ile yeni oturum bilgisi alır
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IDataResult<UserSignInResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        return ReturnResult( await _authService.RefreshTokenAsync(refreshToken, cancellationToken));
    }

    /// <summary>
    /// Oturumu sonlandırır, refresh token ve session'ı siler
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(Cloudot.Shared.Results.IResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        return ReturnResult(await _authService.LogoutAsync(refreshToken, cancellationToken));
    }
}