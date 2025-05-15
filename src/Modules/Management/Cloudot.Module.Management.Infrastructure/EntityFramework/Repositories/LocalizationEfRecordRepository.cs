using Cloudot.Module.Management.Domain.LocalizationRecord;
using Cloudot.Shared.EntityFramework;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Repositories;

public class LocalizationEfRecordRepository(ManagementDbContext context)
    : EfRepository<Domain.LocalizationRecord.LocalizationRecord>(context), ILocalizationEfRecordRepository
{
}