using Cloudot.Shared.DTOs;

namespace Cloudot.Module.Management.Application.Dtos.Tenant;

public class TenantCreateDto : BaseDto
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
}