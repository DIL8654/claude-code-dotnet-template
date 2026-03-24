using Application.Abstractions;
using Domain.Tenants;

namespace Application.Tenants;

public sealed class TenantService(
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<TenantDto> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Tenant tenant = Tenant.Create(
            request.TenantName,
            request.Description,
            request.Slug,
            request.Metadata,
            clock.UtcNow);

        await tenantRepository.AddAsync(tenant, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return tenant.ToDto();
    }
}
