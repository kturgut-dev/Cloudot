namespace Cloudot.Shared.Extensions;

public static class GuidExtensions
{
    public static bool IsEmpty(this Guid guid) => guid == Guid.Empty;
}
