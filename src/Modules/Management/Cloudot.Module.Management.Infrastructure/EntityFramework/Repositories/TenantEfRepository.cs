using Cloudot.Module.Management.Domain.Tenant;
using Cloudot.Shared.EntityFramework;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Repositories;

public class TenantEfRepository(ManagementDbContext context)
    : EfRepository<Tenant, ManagementDbContext>(context), ITenantEfRepository
{
}