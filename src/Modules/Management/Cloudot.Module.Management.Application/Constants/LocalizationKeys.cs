namespace Cloudot.Module.Management.Application.Constants;

public static class LocalizationKeys
{
    private const string BasePrefix = "Cloudot.Module.Management.";

    public static class UserKeys
    {
        public const string NotFound = BasePrefix + "User.NotFound";
        public const string AlreadyExists = BasePrefix + "User.AlreadyExists";
        public const string InvalidEmail = BasePrefix + "User.InvalidEmail";
        public const string InvalidPassword = BasePrefix + "User.InvalidPassword";
        public const string Created = BasePrefix + "User.Created";
        public const string Updated = BasePrefix + "User.Updated";
        public const string Deleted = BasePrefix + "User.Deleted";
        public const string RecordNotActive = BasePrefix + "User.RecordNotActive";
        public const string MailNotVerified = BasePrefix + "User.MailNotVerified";
    }

    public static class AuthKeys
    {
        public const string OtpInvalid = BasePrefix + "Auth.OtpInvalid";
        public const string OtpSent = BasePrefix + "Auth.OtpSent";
        public const string OtpMailError = BasePrefix + "Auth.OtpMailError";
        public const string LoginSuccess = BasePrefix + "Auth.LoginSuccess";
        public const string RefreshTokenInvalid = BasePrefix + "Auth.RefreshTokenInvalid";
        public const string TokenRefreshed = BasePrefix + "Auth.TokenRefreshed";
        public const string SessionClosed = BasePrefix + "Auth.SessionClosed";
        public const string OtpVerified = BasePrefix + "Auth.OtpVerified";
    }

    public static class TenantKeys
    {
        public const string NotFound = BasePrefix + "Tenant.NotFound";
        public const string AlreadyExists = BasePrefix + "Tenant.AlreadyExists";
        public const string ShortNameAlreadyExists = BasePrefix + "Tenant.ShortNameAlreadyExists";
        public const string Created = BasePrefix + "Tenant.Created";
    }
}