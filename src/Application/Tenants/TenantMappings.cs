using Domain.Tenants;

namespace Application.Tenants;

public static class TenantMappings
{
    public static TenantDto ToDto(this Tenant tenant) =>
        new(
            tenant.Id,
            tenant.TenantName,
            tenant.Description,
            tenant.Slug,
            tenant.Metadata,
            tenant.CreatedUtc);
}
