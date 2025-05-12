namespace Cloudot.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);
    
    public static bool IsValidEmail(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        try
        {
            var email = new System.Net.Mail.MailAddress(value);
            return email.Address == value;
        }
        catch
        {
            return false;
        }
    }
}
