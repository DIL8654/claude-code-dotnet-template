namespace Application.Tenants;

public sealed record CreateTenantRequest(
    string TenantName,
    string? Description,
    string Slug,
    string? Metadata);
