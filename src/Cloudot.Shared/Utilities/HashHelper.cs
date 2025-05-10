namespace Cloudot.Shared.Utilities;

public static class HashHelper
{
    public static string ComputeSha256(string rawData)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes);
    }
}