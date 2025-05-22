using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cloudot.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);
    
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
    
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
    
    public static string? ToBase64(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }
    
    public static string Format(this string value, params object[] args)
    {
        return string.Format(value, args);
    }
    
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // 1. Küçük harfe çevir
        text = text.ToLowerInvariant();

        // 2. Türkçe karakter düzeltmeleri (önce)
        text = text.Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ç", "c")
            .Replace("ğ", "g");

        // 3. Unicode normalize (é → e, â → a)
        string normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (char c in normalized)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }
        text = builder.ToString().Normalize(NormalizationForm.FormC);

        // 4. Geçersiz karakterleri temizle
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

        // 5. Boşlukları tireye çevir, birden fazlaları sadeleştir
        text = Regex.Replace(text, @"\s+", "-");
        text = Regex.Replace(text, @"-+", "-");

        // 6. Başta/sonda tire olmasın
        return text.Trim('-');
    }
    
    public static string NormalizeForDb(this string slug) =>
        Regex.Replace(slug.ToLowerInvariant(), @"[^a-z0-9_]", "_");

}
