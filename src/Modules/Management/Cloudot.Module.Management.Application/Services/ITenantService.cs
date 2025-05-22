using Cloudot.Module.Management.Application.Dtos.Tenant;
using Cloudot.Shared.Results;

namespace Cloudot.Module.Management.Application.Services;

public interface ITenantService
{
    Task<IResult> CreateAsync(TenantCreateDto dto, CancellationToken cancellationToken = default);
}