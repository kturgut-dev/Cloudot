namespace Cloudot.Core.Utilities.Caching;

public static class CacheKeyHelper
{
    public static string User(string userId) => $"user:{userId}";
    public static string Tenant(string tenantId) => $"tenant:{tenantId}";
}