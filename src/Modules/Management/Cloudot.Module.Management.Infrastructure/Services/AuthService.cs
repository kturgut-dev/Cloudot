using System.Security.Claims;
using AutoMapper;
using Cloudot.Core.Utilities.Caching;
using Cloudot.Core.Utilities.Security.Sessions;
using Cloudot.Core.Utilities.Security.Tokens;
using Cloudot.Infrastructure.Messaging.Email;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.Exceptions;
using Cloudot.Shared.Extensions;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cloudot.Module.Management.Infrastructure.Services;

public class AuthService(
    ILogger<UserService> _logger,
    // IMapper _mapper,
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IEmailSender _emailSender,
    ICacheManager _cacheManager,
    IRefreshTokenStore _refreshTokenStore,
    ISessionManager _sessionManager,
    IHttpContextAccessor _httpContextAccessor) : IAuthService
{
    private async Task<User> ValidateSignInUserAsync(string email)
    {
        User? user = await _userRepository.GetAsync(x => x.Email == email);

        if (user is null)
        {
            _logger.LogWarning("Kullanıcı bulunamadı: {Email}", email);
            throw new NotFoundAppException("Kullanıcı bulunamadı.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Kullanıcı aktif değil: {Email}", email);
            throw new UnauthorizedAppException("Kullanıcı aktif değil.");
        }

        if (!user.IsMailVerified)
        {
            _logger.LogWarning("Kullanıcı e-posta doğrulaması yapılmamış: {Email}", email);
            throw new UnauthorizedAppException("E-posta doğrulaması yapılmamış.");
        }

        return user;
    }

    public async Task<IResult> RequestOtpAsync(UserSignInDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw new ArgumentException("E-posta formatı geçersiz.");

        User? user = await ValidateSignInUserAsync(dto.Email);
        
        string otpCode = new Random().Next(100000, 999999).ToString();
        string cacheKey = $"otp:signin:{dto.Email}";
        
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
            throw new AppException("E-posta gönderim hatası.");
        }

        return Result.Success("OTP gönderildi");
    }

    public async Task<IDataResult<UserSignInResponse>> VerifyOtpAndSignInAsync(UserVerifyOtpDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw new ValidationAppException("E-posta formatı geçersiz.");

        string cacheKey = $"otp:signin:{dto.Email}";
        string? cachedOtp = await _cacheManager.GetAsync<string>(cacheKey);

        if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.OtpCode)
        {
            _logger.LogWarning("Geçersiz veya süresi geçmiş OTP: {Email}", dto.Email);
            throw new UnauthorizedAppException("OTP doğrulaması başarısız.");
        }

        User? user = await ValidateSignInUserAsync(dto.Email);
        
        await _cacheManager.RemoveAsync(cacheKey); // OTP'yi temizle

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Refresh Token ve Session oluştur
        string refreshToken = Guid.NewGuid().ToString("N");
        DateTime now = DateTime.UtcNow;
        DateTime refreshExpiration = now.AddDays(7);

        string ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
        string userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"];

        await _refreshTokenStore.StoreAsync(new RefreshTokenInfo
        {
            Token = refreshToken,
            UserId = user.Id,
            IpAddress = ip,
            UserAgent = userAgent,
            CreatedAt = now,
            Expiration = refreshExpiration
        });

        await _sessionManager.SetAsync(user.Id, new SessionInfo
        {
            UserId = user.Id,
            Email = user.Email,
            IpAddress = ip,
            UserAgent = userAgent,
            LoginTime = now
        });

        UserSignInResponse response = new()
        {
            Email = user.Email,
            RefreshToken = refreshToken,
            Expiration = now.AddHours(8)
        };

        _logger.LogInformation("Kullanıcı başarılı şekilde giriş yaptı: {Email}", user.Email);
        return DataResult<UserSignInResponse>.Success(response, "Giriş başarılı");
    }

    public async Task<IResult> SignUpAsync(UserSignUpDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw new ValidationAppException("E-posta formatı geçersiz.");

        User? existingUser = await _userRepository.GetAsync(x => x.Email == dto.Email);
        if (existingUser is not null)
            return Result.Fail("Bu e-posta ile zaten kayıt olunmuş.");

        User user = new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IsActive = false,
            IsMailVerified = false
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        string otpCode = new Random().Next(100000, 999999).ToString();
        string cacheKey = $"otp:signup:{dto.Email}";
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

        return Result.Success("OTP gönderildi, lütfen e-postanızı doğrulayın.");
    }

    public async Task<IResult> VerifySignUpOtpAsync(UserVerifyOtpDto dto)
    {
        string cacheKey = $"otp:signup:{dto.Email}";
        string? cachedOtp = await _cacheManager.GetAsync<string>(cacheKey);

        if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.OtpCode)
            return Result.Fail("OTP doğrulaması başarısız.");

        User? user = await _userRepository.GetAsync(x => x.Email == dto.Email);
        if (user is null)
            return Result.Fail("Kullanıcı bulunamadı.");

        user.IsActive = true;
        user.IsMailVerified = true;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _cacheManager.RemoveAsync(cacheKey);

        _logger.LogInformation("Kullanıcı başarıyla doğrulandı: {Email}", user.Email);
        return Result.Success("Hesabınız başarıyla doğrulandı.");
    }
    
    public async Task<IDataResult<UserSignInResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        RefreshTokenInfo? storedToken = await _refreshTokenStore.GetAsync(refreshToken, cancellationToken);
        if (storedToken is null || storedToken.Expiration <= DateTime.UtcNow)
            throw new UnauthorizedAppException("Geçersiz veya süresi dolmuş refresh token.");

        User? user = await _userRepository.GetAsync(x => x.Id == storedToken.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundAppException("Kullanıcı bulunamadı.");

        // Yeni token üret
        string newRefreshToken = Guid.NewGuid().ToString("N");
        DateTime now = DateTime.UtcNow;

        await _refreshTokenStore.StoreAsync(new RefreshTokenInfo
        {
            Token = newRefreshToken,
            UserId = user.Id,
            IpAddress = storedToken.IpAddress,
            UserAgent = storedToken.UserAgent,
            CreatedAt = now,
            Expiration = now.AddDays(7)
        });

        await _refreshTokenStore.RemoveAsync(refreshToken, cancellationToken); // eskiyi sil

        UserSignInResponse response = new()
        {
            Email = user.Email,
            RefreshToken = newRefreshToken,
            Expiration = now.AddHours(8)
        };

        return DataResult<UserSignInResponse>.Success(response, "Token yenilendi");
    }

    public async Task<IResult> LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        await _refreshTokenStore.RemoveAsync(refreshToken, cancellationToken);
        await _sessionManager.RemoveAsync(userId, cancellationToken);
        return Result.Success("Oturum sonlandırıldı.");
    }

}