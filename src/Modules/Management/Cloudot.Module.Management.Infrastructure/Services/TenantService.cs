using AutoMapper;
using Cloudot.Core.Utilities.Caching;
using Cloudot.Infrastructure.Auth;
using Cloudot.Module.Management.Application.Dtos.Tenant;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.Tenant;
using Cloudot.Shared.Exceptions;
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
        Tenant tenant = _mapper.Map<Tenant>(dto);
        tenant.OwnerId = _currentUser.Id;

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_localizer["Tenant Created"]);
    }
}