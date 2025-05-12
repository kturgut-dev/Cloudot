using System.Security.Claims;
using AutoMapper;
using Cloudot.Core.Utilities.Caching;
using Cloudot.Infrastructure.Messaging.Email;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.Extensions;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Results;
using Microsoft.Extensions.Logging;

namespace Cloudot.Module.Management.Infrastructure.Services;

public class AuthService(
    ILogger<UserService> _logger,
    // IMapper _mapper,
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IEmailSender _emailSender,
    ICacheManager _cacheManager) : IAuthService
{
    public async Task<IResult> RequestOtpAsync(UserSignInDto userSignInDto)
    {
        if (!userSignInDto.Email.IsValidEmail())
            throw new ArgumentException("E-posta formatı geçersiz.");

        User? user = await _userRepository.GetAsync(x => x.Email == userSignInDto.Email);
        if (user is null)
        {
            _logger.LogWarning("Kullanıcı bulunamadı: {Email}", userSignInDto.Email);
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");
        }
        else if (!user.IsActive)
        {
            _logger.LogWarning("Kullanıcı aktif değil: {Email}", userSignInDto.Email);
            throw new UnauthorizedAccessException("Kullanıcı aktif değil.");
        }
        if (user.IsMailVerified)
        {
            _logger.LogWarning("Kullanıcı e-posta doğrulaması yapılmamış: {Email}", userSignInDto.Email);
            throw new UnauthorizedAccessException("E-posta doğrulaması yapılmamış.");
        }

        string otpCode = new Random().Next(100000, 999999).ToString();
        string cacheKey = $"otp:{userSignInDto.Email}";
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
            throw new Exception("E-posta gönderim hatası.");
        }

        return Result.Success("OTP gönderildi");
    }
    public async Task<IDataResult<UserSignInResponse>> VerifyOtpAndSignInAsync(UserVerifyOtpDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw new ArgumentException("E-posta formatı geçersiz.");

        string cacheKey = $"otp:{dto.Email}";
        string? cachedOtp = await _cacheManager.GetAsync<string>(cacheKey);

        if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.OtpCode)
        {
            _logger.LogWarning("Geçersiz veya süresi geçmiş OTP: {Email}", dto.Email);
            throw new UnauthorizedAccessException("OTP doğrulaması başarısız.");
        }

        User? user = await _userRepository.GetAsync(x => x.Email == dto.Email);
        if (user is null)
        {
            _logger.LogWarning("Kullanıcı bulunamadı (OTP sonrası): {Email}", dto.Email);
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");
        }

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // string token = _tokenHelper.GenerateToken(claims);
        // string refreshToken = Guid.NewGuid().ToString();

        UserSignInResponse response = new UserSignInResponse
        {
            Email = user.Email,
            // Token = token,
            // RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddHours(8)
        };

        _logger.LogInformation("Kullanıcı başarılı şekilde giriş yaptı: {Email}", user.Email);
        return DataResult<UserSignInResponse>.Success(response, "Giriş başarılı");
    }
    public async Task<IResult> SignUpAsync(UserSignUpDto dto)
    {
        if (!dto.Email.IsValidEmail())
            throw new ArgumentException("E-posta formatı geçersiz.");

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
}