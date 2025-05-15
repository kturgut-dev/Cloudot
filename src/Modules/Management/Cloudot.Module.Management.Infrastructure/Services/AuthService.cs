using Cloudot.Core.Utilities.Caching;
using Cloudot.Core.Utilities.Security.Sessions;
using Cloudot.Core.Utilities.Security.Tokens;
using Cloudot.Infrastructure.Auth;
using Cloudot.Infrastructure.Auth.Jwt;
using Cloudot.Infrastructure.Messaging.Email;
using Cloudot.Module.Management.Application.Constants;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.Exceptions;
using Cloudot.Shared.Extensions;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using IResult = Cloudot.Shared.Results.IResult;

namespace Cloudot.Module.Management.Infrastructure.Services;

public class AuthService(
    ILogger<UserService> _logger,
    // IMapper _mapper,
    ICurrentUser _currentUser,
    IJwtTokenHelper _jwtTokenHelper,
    IUserEfRepository _userEfRepository,
    IUnitOfWork _unitOfWork,
    IEmailSender _emailSender,
    ICacheManager _cacheManager,
    IRefreshTokenStore _refreshTokenStore,
    ISessionManager _sessionManager,
    IHttpContextAccessor _httpContextAccessor,
    IStringLocalizer<AuthService> _localizer,
    IExceptionFactory _exceptionFactory) : IAuthService
{
    private async Task<User> ValidateSignInUserAsync(string email)
    {
        User? user = await _userEfRepository.GetAsync(x => x.Email == email);

        if (user is null)
        {
            _logger.LogWarning("Kullanıcı bulunamadı: {Email}", email);
            throw _exceptionFactory.NotFound(LocalizationKeys.User.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Kullanıcı aktif değil: {Email}", email);
            throw _exceptionFactory.Unauthorized(LocalizationKeys.User.RecordNotActive);
        }

        if (!user.IsMailVerified)
        {
            _logger.LogWarning("Kullanıcı e-posta doğrulaması yapılmamış: {Email}", email);
            throw _exceptionFactory.Unauthorized(LocalizationKeys.User.MailNotVerified);
        }

        return user;
    }

    public async Task<IResult> RequestOtpAsync(UserSignInDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw _exceptionFactory.Validation(LocalizationKeys.User.InvalidEmail);

        User? user = await ValidateSignInUserAsync(dto.Email);

        string otpCode = new Random().Next(100000, 999999).ToString();
        string cacheKey = CacheKeys.Auth.OtpSignIn.Format(dto.Email); //$"otp:signin:{dto.Email}";

        await _cacheManager.RemoveAsync(cacheKey); // Önceki OTP'yi temizle
        await _cacheManager.SetAsync(cacheKey, otpCode, TimeSpan.FromMinutes(5));

        _logger.LogInformation("OTP oluşturuldu ve cache'e eklendi: {Email}, OTP: {Otp}", user.Email, otpCode);

        EmailMessage emailMessage = new()
        {
            To = user.Email,
            Subject = "Giriş için OTP Kodu",
            Body = $"Giriş yapmak için OTP kodunuz: {otpCode}",
            IsHtml = true
        };

        try
        {
            await _emailSender.SendAsync(emailMessage);
            _logger.LogInformation("OTP e-posta gönderildi: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "E-posta gönderim hatası: {Email}", user.Email);
            throw _exceptionFactory.Create(LocalizationKeys.Auth.OtpMailError);
        }

        return Result.Success(_localizer[LocalizationKeys.Auth.OtpSent]);
    }

    public async Task<IDataResult<UserSignInResponse>> VerifyOtpAndSignInAsync(UserVerifyOtpDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw _exceptionFactory.Validation(LocalizationKeys.User.InvalidEmail);

        string cacheKey = CacheKeys.Auth.OtpSignIn.Format(dto.Email); //$"otp:signin:{dto.Email}";
        string? cachedOtp = await _cacheManager.GetAsync<string>(cacheKey);

        if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.OtpCode)
        {
            _logger.LogWarning("Geçersiz veya süresi geçmiş OTP: {Email}", dto.Email);
            throw _exceptionFactory.Unauthorized(LocalizationKeys.Auth.OtpInvalid);
        }

        User? user = await ValidateSignInUserAsync(dto.Email);
        await _cacheManager.RemoveAsync(cacheKey); // OTP'yi temizle

        // JWT Token oluştur (access + refresh)
        JwtTokenResponse jwtTokens = _jwtTokenHelper.CreateToken(user.Id, user.Email);

        // Refresh token'ı kaydet
        DateTime now = DateTime.UtcNow;
        string ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
        string userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

        await _refreshTokenStore.StoreAsync(new RefreshTokenInfo
        {
            Token = jwtTokens.RefreshToken,
            UserId = user.Id,
            IpAddress = ip,
            UserAgent = userAgent ?? string.Empty,
            CreatedAt = now,
            Expiration = jwtTokens.RefreshTokenExpiresAt
        });

        // Session kaydet
        await _sessionManager.SetAsync(user.Id, new SessionInfo
        {
            UserId = user.Id,
            Email = user.Email,
            IpAddress = ip,
            UserAgent = userAgent ?? string.Empty,
            LoginTime = now
        });

        // Response DTO hazırla
        UserSignInResponse response = new()
        {
            Email = user.Email,
            AccessToken = jwtTokens.AccessToken,
            RefreshToken = jwtTokens.RefreshToken,
            Expiration = jwtTokens.AccessTokenExpiresAt
        };

        _logger.LogInformation("Kullanıcı başarılı şekilde giriş yaptı: {Email}", user.Email);
        return DataResult<UserSignInResponse>.Success(response, _localizer[LocalizationKeys.Auth.LoginSuccess]);
    }

    public async Task<IResult> SignUpAsync(UserSignUpDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw _exceptionFactory.Validation(LocalizationKeys.User.InvalidEmail);

        User? existingUser = await _userEfRepository.GetAsync(x => x.Email == dto.Email);
        if (existingUser is not null)
            return Result.Fail(_localizer[LocalizationKeys.User.AlreadyExists]);

        User user = new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IsActive = false,
            IsMailVerified = false
        };

        await _userEfRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        string otpCode = new Random().Next(100000, 999999).ToString();
        string cacheKey = CacheKeys.Auth.OtpSignUp.Format(dto.Email); //$"otp:signup:{dto.Email}";
        await _cacheManager.SetAsync(cacheKey, otpCode, TimeSpan.FromMinutes(10));

        EmailMessage emailMessage = new()
        {
            To = dto.Email,
            Subject = "Kayıt için OTP Kodu",
            Body = $"Kayıt işlemini tamamlamak için OTP kodunuz: {otpCode}",
            IsHtml = true
        };

        await _emailSender.SendAsync(emailMessage);
        _logger.LogInformation("Kayıt OTP e-postası gönderildi: {Email}", dto.Email);

        return Result.Success(_localizer[LocalizationKeys.Auth.OtpSent]);
    }

    public async Task<IResult> VerifySignUpOtpAsync(UserVerifyOtpDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw _exceptionFactory.Validation(LocalizationKeys.User.InvalidEmail);

        if (string.IsNullOrEmpty(dto.OtpCode))
            throw _exceptionFactory.Validation(LocalizationKeys.User.InvalidPassword);

        string cacheKey = CacheKeys.Auth.OtpSignUp.Format(dto.Email); // $"otp:signup:{dto.Email}";
        string? cachedOtp = await _cacheManager.GetAsync<string>(cacheKey);

        if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.OtpCode)
            return Result.Fail(_localizer[LocalizationKeys.Auth.OtpInvalid]);

        User? user = await _userEfRepository.GetAsync(x => x.Email == dto.Email);
        if (user is null)
            return Result.Fail(_localizer[LocalizationKeys.User.NotFound]);

        user.IsActive = true;
        user.IsMailVerified = true;

        await _userEfRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _cacheManager.RemoveAsync(cacheKey);

        _logger.LogInformation("Kullanıcı başarıyla doğrulandı: {Email}", user.Email);
        return Result.Success(_localizer[LocalizationKeys.Auth.OtpVerified]);
    }

    public async Task<IDataResult<UserSignInResponse>> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        RefreshTokenInfo? storedToken = await _refreshTokenStore.GetAsync(refreshToken, cancellationToken);
        if (storedToken is null || storedToken.Expiration <= DateTime.UtcNow)
            throw _exceptionFactory.Unauthorized(LocalizationKeys.Auth.OtpInvalid);

        User? user = await _userEfRepository.GetAsync(x => x.Id == storedToken.UserId, cancellationToken);
        if (user is null)
            throw _exceptionFactory.NotFound(LocalizationKeys.User.NotFound);

        // Yeni JWT ve refresh token üret
        JwtTokenResponse jwtTokens = _jwtTokenHelper.CreateToken(user.Id, user.Email);

        DateTime now = DateTime.UtcNow;

        // Yeni refresh token'ı kaydet
        await _refreshTokenStore.StoreAsync(new RefreshTokenInfo
        {
            Token = jwtTokens.RefreshToken,
            UserId = user.Id,
            IpAddress = storedToken.IpAddress,
            UserAgent = storedToken.UserAgent,
            CreatedAt = now,
            Expiration = jwtTokens.RefreshTokenExpiresAt
        });

        // Eski refresh token'ı sil
        await _refreshTokenStore.RemoveAsync(refreshToken, cancellationToken);

        // Yeni token bilgilerini response'a aktar
        UserSignInResponse response = new()
        {
            Email = user.Email,
            AccessToken = jwtTokens.AccessToken,
            RefreshToken = jwtTokens.RefreshToken,
            Expiration = jwtTokens.AccessTokenExpiresAt
        };

        _logger.LogInformation("Refresh token başarıyla yenilendi: {Email}", user.Email);
        return DataResult<UserSignInResponse>.Success(response, _localizer[LocalizationKeys.Auth.TokenRefreshed]);
    }

    public async Task<IResult> LogoutAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        Guid? userId = _currentUser.Id;
        if (userId is null)
            throw _exceptionFactory.Unauthorized(LocalizationKeys.User.NotFound);

        // Refresh token varsa sil
        RefreshTokenInfo? token = await _refreshTokenStore.GetAsync(refreshToken, cancellationToken);
        if (token is not null)
        {
            await _refreshTokenStore.RemoveAsync(refreshToken, cancellationToken);
            _logger.LogInformation("Refresh token silindi: {UserId}, Token: {RefreshToken}", userId, refreshToken);
        }
        else
        {
            _logger.LogWarning("Silinmek istenen refresh token bulunamadı: {RefreshToken}", refreshToken);
        }

        // Session verisini temizle
        await _sessionManager.RemoveAsync(userId!.Value, cancellationToken);
        _logger.LogInformation("Kullanıcı oturumu sonlandırıldı: {UserId}", userId);

        return Result.Success(_localizer[LocalizationKeys.Auth.SessionClosed]);
    }
}