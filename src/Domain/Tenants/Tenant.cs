using Domain.Abstractions;

namespace Domain.Tenants;

public sealed class Tenant : AuditableEntity
{
    private Tenant()
    {
    }

    private Tenant(string tenantName, string? description, string slug, string? metadata, DateTimeOffset createdUtc)
    {
        TenantName = tenantName;
        Description = description;
        Slug = slug;
        Metadata = metadata;
        CreatedUtc = createdUtc;
    }

    public string TenantName { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string Slug { get; private set; } = string.Empty;

    public string? Metadata { get; private set; }

    public static Tenant Create(
        string tenantName,
        string? description,
        string slug,
        string? metadata,
        DateTimeOffset createdUtc)
    {
        if (string.IsNullOrWhiteSpace(tenantName))
        {
            throw new ArgumentException("Tenant name is required.", nameof(tenantName));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Slug is required.", nameof(slug));
        }

        return new Tenant(tenantName.Trim(), description?.Trim(), slug, metadata, createdUtc);
    }
}
