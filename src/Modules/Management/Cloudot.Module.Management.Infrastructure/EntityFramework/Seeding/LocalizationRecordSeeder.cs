using Cloudot.Module.Management.Application.Constants;
using Cloudot.Module.Management.Domain.LocalizationRecord;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.EntityFramework.Seeding;
using Cloudot.Shared.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Seeding;

public class LocalizationRecordSeeder : ISeeder
{
    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IEfRepository<LocalizationRecord> repository = scope.ServiceProvider.GetRequiredService<IEfRepository<LocalizationRecord>>();

        // Daha önce seed edilmişse tekrar etme
        bool exists = await repository.AnyAsync(x => x.Key.StartsWith("Cloudot.Module.Management") && x.Culture == "tr-TR");
        if (exists)
            return;

        List<LocalizationRecord> records = new List<LocalizationRecord>
        {
            // 🌐 tr-TR
            new("tr-TR", LocalizationKeys.User.NotFound, "Kullanıcı bulunamadı"),
            new("tr-TR", LocalizationKeys.User.AlreadyExists, "Kullanıcı zaten mevcut"),
            new("tr-TR", LocalizationKeys.User.InvalidEmail, "Geçersiz e-posta"),
            new("tr-TR", LocalizationKeys.User.InvalidPassword, "Geçersiz şifre"),
            new("tr-TR", LocalizationKeys.User.Created, "Kullanıcı oluşturuldu"),
            new("tr-TR", LocalizationKeys.User.Updated, "Kullanıcı güncellendi"),
            new("tr-TR", LocalizationKeys.User.Deleted, "Kullanıcı silindi"),
            new("tr-TR", LocalizationKeys.User.RecordNotActive, "Kullanıcı aktif değil"),
            new("tr-TR", LocalizationKeys.User.MailNotVerified, "E-posta doğrulanmamış"),

            new("tr-TR", LocalizationKeys.Auth.OtpInvalid, "OTP kodu geçersiz"),
            new("tr-TR", LocalizationKeys.Auth.OtpSent, "OTP gönderildi"),
            new("tr-TR", LocalizationKeys.Auth.OtpMailError, "OTP gönderilirken hata oluştu"),
            new("tr-TR", LocalizationKeys.Auth.LoginSuccess, "Giriş başarılı"),
            new("tr-TR", LocalizationKeys.Auth.RefreshTokenInvalid, "Yenileme anahtarı geçersiz"),
            new("tr-TR", LocalizationKeys.Auth.TokenRefreshed, "Token yenilendi"),
            new("tr-TR", LocalizationKeys.Auth.SessionClosed, "Oturum kapatıldı"),
            new("tr-TR", LocalizationKeys.Auth.OtpVerified, "OTP doğrulandı"),

            // 🌐 en-US
            new("en-US", LocalizationKeys.User.NotFound, "User not found"),
            new("en-US", LocalizationKeys.User.AlreadyExists, "User already exists"),
            new("en-US", LocalizationKeys.User.InvalidEmail, "Invalid email"),
            new("en-US", LocalizationKeys.User.InvalidPassword, "Invalid password"),
            new("en-US", LocalizationKeys.User.Created, "User created"),
            new("en-US", LocalizationKeys.User.Updated, "User updated"),
            new("en-US", LocalizationKeys.User.Deleted, "User deleted"),
            new("en-US", LocalizationKeys.User.RecordNotActive, "User is not active"),
            new("en-US", LocalizationKeys.User.MailNotVerified, "Email is not verified"),

            new("en-US", LocalizationKeys.Auth.OtpInvalid, "OTP code is invalid"),
            new("en-US", LocalizationKeys.Auth.OtpSent, "OTP sent"),
            new("en-US", LocalizationKeys.Auth.OtpMailError, "Error sending OTP"),
            new("en-US", LocalizationKeys.Auth.LoginSuccess, "Login successful"),
            new("en-US", LocalizationKeys.Auth.RefreshTokenInvalid, "Refresh token is invalid"),
            new("en-US", LocalizationKeys.Auth.TokenRefreshed, "Token refreshed"),
            new("en-US", LocalizationKeys.Auth.SessionClosed, "Session closed"),
            new("en-US", LocalizationKeys.Auth.OtpVerified, "OTP verified")
        };

        await repository.AddRangeAsync(records);
        await unitOfWork.SaveChangesAsync();
    }
}