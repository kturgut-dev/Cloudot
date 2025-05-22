using Cloudot.Module.Management.Domain.Tenant;
using Cloudot.Shared.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Configurations;

public class TenantConfiguration : BaseAuditEntityConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Domain)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.TimeZone)
            .IsRequired()
            .HasMaxLength(100); // örn: "Turkey Standard Time"

        builder.Property(x => x.DatabaseName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.EnableSupport)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.OwnerId)
            .IsRequired(false);

        builder.Property(x => x.PlanId)
            .IsRequired(false);

        builder.HasOne(x => x.Owner)
            .WithMany(x => x.Tenants)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(x => new { x.ShortName, x.OwnerId })
            .IsUnique();
    }
}