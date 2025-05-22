using Cloudot.Module.Management.Application.Dtos.Tenant;
using Cloudot.Module.Management.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cloudot.WebAPI.Controllers.Management;

[ApiController]
[Route("api/[controller]")]
public class TenantController(ITenantService tenantService) : MainController
{
    /// <summary>
    /// Tenant oluşturur
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(Cloudot.Shared.Results.IResult), 200)]
    [Validate]
    public async Task<IActionResult> Create([FromBody] TenantCreateDto dto)
    {
        return ReturnResult(await tenantService.CreateAsync(dto));
    }
}