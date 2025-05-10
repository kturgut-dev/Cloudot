namespace Cloudot.Module.Management.Infrastructure;

// public class EfTenantStore(ManagementDbContext context) : ITenantStore
// {
//     public async Task<TenantConnectionInfo?> GetConnectionInfoByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
//     {
//         return await context.Users
//             .Where(u => u.Id == userId)
//             .Select(u => new TenantConnectionInfo
//             {
//                 Id = u.TenantId,
//                 DbName = u.Tenant.DbName
//             })
//             .FirstOrDefaultAsync(cancellationToken);
//     }
// }
