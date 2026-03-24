namespace Application.Tenants;

public sealed record TenantDto(
    Guid Id,
    string TenantName,
    string? Description,
    string Slug,
    string? Metadata,
    DateTimeOffset CreatedUtc);
