namespace Cloudot.Module.Management.Application.Constants;

public class CacheKeys
{
    private const string BasePrefix = "Management:";

    public static class Auth
    {
        public const string OtpSignIn = BasePrefix + "Auth:Otp:SignIn:{0}";
        public const string OtpSignUp = BasePrefix + "Auth:Otp:SignUp:{0}";
        public const string OtpForgotPassword = BasePrefix + "Auth:Otp:ForgotPassword:{0}";
    }
}