using AutoMapper;
using Cloudot.Core.Utilities.Caching;
using Cloudot.Infrastructure.Auth;
using Cloudot.Module.Management.Application.Dtos.Tenant;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.Events;
using Cloudot.Module.Management.Domain.Tenant;
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
    IUnitOfWork _unitOfWork,
    ICacheManager _cacheManager,
    IStringLocalizer<TenantService> _localizer,
    IExceptionFactory _exceptionFactory) : ITenantService
{
    public async Task<IResult> CreateAsync(TenantCreateDto dto, CancellationToken cancellationToken = default)
    {
        string shortName = dto.ShortName.IsNullOrWhiteSpace()
            ? dto.Name.ToSlug()
            : dto.ShortName.ToSlug();
        
        // ShortName tek mi kontrol et
        bool isShortNameExists = await _tenantRepository.AnyAsync(x =>
            x.ShortName == shortName && x.OwnerId == _currentUser.Id, cancellationToken);

        if (isShortNameExists)
            return Result.Fail(_localizer["Tenant.ShortNameAlreadyExists"]);

        // Mapleme
        Tenant tenant = _mapper.Map<Tenant>(dto);

        // DatabaseName üret
        tenant.DatabaseName = $"cloudot_tenant_{shortName.NormalizeForDb()}_db";
        
        tenant.AddDomainEvent(new TenantCreatedEvent(tenant.Id, tenant.DatabaseName));

        tenant.ShortName = shortName;
        tenant.OwnerId = _currentUser.Id;
        tenant.IsActive = true;
        tenant.EnableSupport = true;

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_localizer["Tenant Created"]);
    }
}