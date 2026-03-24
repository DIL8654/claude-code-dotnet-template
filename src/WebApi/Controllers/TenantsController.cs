using Application.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Contracts;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/tenants")]
public sealed class TenantsController(TenantService tenantService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CreateTenant)]
    [ProducesResponseType<TenantDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TenantDto>> CreateAsync(
        [FromBody] CreateTenantHttpRequest request,
        CancellationToken cancellationToken)
    {
        TenantDto created = await tenantService.CreateAsync(
            new CreateTenantRequest(
                request.TenantName,
                request.Description,
                request.Slug,
                request.Metadata?.GetRawText()),
            cancellationToken);

        return Created($"/api/v1/tenants/{created.Id}", created);
    }
}
