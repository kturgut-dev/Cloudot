namespace Cloudot.Core.Utilities.Caching;

public static class CacheKeyHelper
{
    public static string User(Guid userId) => $"user:{userId}";
    public static string Tenant(Guid tenantId) => $"tenant:{tenantId}";
}