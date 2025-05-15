using Cloudot.Shared.Repository.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocalizationRecord = Cloudot.Module.Management.Domain.LocalizationRecord.LocalizationRecord;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Configurations;

public class LocalizationRecordConfiguration : BaseEntityConfiguration<Domain.LocalizationRecord.LocalizationRecord>
{
    public override void Configure(EntityTypeBuilder<LocalizationRecord> builder)
    {
        base.Configure(builder);
        
        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Key)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();

    }
}