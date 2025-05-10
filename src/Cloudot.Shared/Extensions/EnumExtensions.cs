namespace Cloudot.Shared.Extensions;

public static class EnumExtensions
{
    public static string ToDisplayName(this Enum value) =>
        value.ToString();
}