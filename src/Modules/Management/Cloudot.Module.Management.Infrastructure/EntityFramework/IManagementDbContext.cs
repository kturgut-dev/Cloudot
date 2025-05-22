using Cloudot.Module.Management.Domain.LocalizationRecord;
using Cloudot.Module.Management.Domain.Tenant;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework;

public interface IManagementDbContext : IBaseDbContext
{
    DbSet<User> Users { get; }
    DbSet<LocalizationRecord> LocalizationRecords { get; }
    DbSet<Tenant> Tenants { get; }
}