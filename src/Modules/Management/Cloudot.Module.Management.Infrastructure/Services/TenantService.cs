using AutoMapper;
using Cloudot.Core.Utilities.Caching;
using Cloudot.Infrastructure.Auth;
using Cloudot.Module.Management.Application.Constants;
using Cloudot.Module.Management.Application.Dtos.Tenant;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.Events;
using Cloudot.Module.Management.Domain.Tenant;
using Cloudot.Module.Management.Infrastructure.EntityFramework;
using Cloudot.Shared.Exceptions;
using Cloudot.Shared.Extensions;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Results;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Cloudot.Module.Management.Infrastructure.Services;

public class TenantService(
    ILogger<TenantService> _logger,
    ITenantEfRepository _tenantRepository,
    IMapper _mapper,
    ICurrentUser _currentUser,
    IUnitOfWork<ManagementDbContext> _unitOfWork,
    ICacheManager _cacheManager,
    IStringLocalizer<TenantService> _localizer,
    IExceptionFactory _exceptionFactory) : ITenantService
{
    public async Task<IResult> CreateAsync(TenantCreateDto dto, CancellationToken cancellationToken = default)
    {
        string shortName = dto.ShortName.IsNullOrWhiteSpace()
            ? dto.Name!.ToSlug()
            : dto.ShortName!.ToSlug();
        
        // ShortName max 40 karakter ile sınırla (dbName + prefix + suffix toplamda 63 olacak şekilde)
        if (shortName.Length > 40)
            shortName = shortName[..40];
        
        // ShortName tek mi kontrol et
        bool isShortNameExists = await _tenantRepository.AnyAsync(x =>
            x.ShortName == shortName && x.OwnerId == _currentUser.Id, cancellationToken);

        if (isShortNameExists)
            return Result.Fail(_localizer[LocalizationKeys.TenantKeys.AlreadyExists]);

        // Mapleme
        Tenant tenant = _mapper.Map<Tenant>(dto);

        // DatabaseName üret
        string rawDbName = $"tenant_{shortName.NormalizeForDb()}_db";
        if (rawDbName.Length > 63)
            rawDbName = rawDbName[..63];
        
        tenant.DatabaseName = rawDbName;
        tenant.ShortName = shortName;
        tenant.OwnerId = _currentUser.Id;
        tenant.IsActive = true;
        tenant.EnableSupport = true;

        tenant.AddDomainEvent(new TenantCreatedEvent(tenant.Id, tenant.DatabaseName));
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_localizer[LocalizationKeys.TenantKeys.Created]);
    }
}