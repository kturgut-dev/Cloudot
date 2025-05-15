using Cloudot.Module.Management.Domain.LocalizationRecord;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Module.Management.Infrastructure.EntityFramework.Configurations;
using Cloudot.Shared.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework;

public class ManagementDbContext(DbContextOptions<ManagementDbContext> options)
    : BaseDbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<LocalizationRecord> LocalizationRecords => Set<LocalizationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}