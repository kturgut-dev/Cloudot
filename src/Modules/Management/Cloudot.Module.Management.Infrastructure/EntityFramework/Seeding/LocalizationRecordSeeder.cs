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
            new("tr-TR", LocalizationKeys.UserKeys.NotFound, "Kullanıcı bulunamadı"),
            new("tr-TR", LocalizationKeys.UserKeys.AlreadyExists, "Kullanıcı zaten mevcut"),
            new("tr-TR", LocalizationKeys.UserKeys.InvalidEmail, "Geçersiz e-posta"),
            new("tr-TR", LocalizationKeys.UserKeys.InvalidPassword, "Geçersiz şifre"),
            new("tr-TR", LocalizationKeys.UserKeys.Created, "Kullanıcı oluşturuldu"),
            new("tr-TR", LocalizationKeys.UserKeys.Updated, "Kullanıcı güncellendi"),
            new("tr-TR", LocalizationKeys.UserKeys.Deleted, "Kullanıcı silindi"),
            new("tr-TR", LocalizationKeys.UserKeys.RecordNotActive, "Kullanıcı aktif değil"),
            new("tr-TR", LocalizationKeys.UserKeys.MailNotVerified, "E-posta doğrulanmamış"),

            new("tr-TR", LocalizationKeys.AuthKeys.OtpInvalid, "OTP kodu geçersiz"),
            new("tr-TR", LocalizationKeys.AuthKeys.OtpSent, "OTP gönderildi"),
            new("tr-TR", LocalizationKeys.AuthKeys.OtpMailError, "OTP gönderilirken hata oluştu"),
            new("tr-TR", LocalizationKeys.AuthKeys.LoginSuccess, "Giriş başarılı"),
            new("tr-TR", LocalizationKeys.AuthKeys.RefreshTokenInvalid, "Yenileme anahtarı geçersiz"),
            new("tr-TR", LocalizationKeys.AuthKeys.TokenRefreshed, "Token yenilendi"),
            new("tr-TR", LocalizationKeys.AuthKeys.SessionClosed, "Oturum kapatıldı"),
            new("tr-TR", LocalizationKeys.AuthKeys.OtpVerified, "OTP doğrulandı"),

            // 🌐 en-US
            new("en-US", LocalizationKeys.UserKeys.NotFound, "User not found"),
            new("en-US", LocalizationKeys.UserKeys.AlreadyExists, "User already exists"),
            new("en-US", LocalizationKeys.UserKeys.InvalidEmail, "Invalid email"),
            new("en-US", LocalizationKeys.UserKeys.InvalidPassword, "Invalid password"),
            new("en-US", LocalizationKeys.UserKeys.Created, "User created"),
            new("en-US", LocalizationKeys.UserKeys.Updated, "User updated"),
            new("en-US", LocalizationKeys.UserKeys.Deleted, "User deleted"),
            new("en-US", LocalizationKeys.UserKeys.RecordNotActive, "User is not active"),
            new("en-US", LocalizationKeys.UserKeys.MailNotVerified, "Email is not verified"),

            new("en-US", LocalizationKeys.AuthKeys.OtpInvalid, "OTP code is invalid"),
            new("en-US", LocalizationKeys.AuthKeys.OtpSent, "OTP sent"),
            new("en-US", LocalizationKeys.AuthKeys.OtpMailError, "Error sending OTP"),
            new("en-US", LocalizationKeys.AuthKeys.LoginSuccess, "Login successful"),
            new("en-US", LocalizationKeys.AuthKeys.RefreshTokenInvalid, "Refresh token is invalid"),
            new("en-US", LocalizationKeys.AuthKeys.TokenRefreshed, "Token refreshed"),
            new("en-US", LocalizationKeys.AuthKeys.SessionClosed, "Session closed"),
            new("en-US", LocalizationKeys.AuthKeys.OtpVerified, "OTP verified")
        };

        await repository.AddRangeAsync(records);
        await unitOfWork.SaveChangesAsync();
    }
}