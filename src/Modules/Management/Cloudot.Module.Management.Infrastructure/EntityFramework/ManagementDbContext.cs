using Cloudot.Module.Management.Domain.LocalizationRecord;
using Cloudot.Module.Management.Domain.Tenant;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Module.Management.Infrastructure.EntityFramework.Configurations;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.EntityFramework.Interceptor;
using Microsoft.EntityFrameworkCore;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework;

public class ManagementDbContext(
    DbContextOptions<ManagementDbContext> options)
    : BaseDbContext(options), IManagementDbContext
{
    public override string SchemaName => "management";
    public DbSet<User> Users => Set<User>();
    public DbSet<LocalizationRecord> LocalizationRecords => Set<LocalizationRecord>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(this.SchemaName);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new LocalizationRecordConfiguration());
    }
}