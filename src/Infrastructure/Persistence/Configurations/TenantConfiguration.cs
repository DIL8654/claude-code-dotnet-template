using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(tenant => tenant.Id);

        builder.Property(tenant => tenant.TenantName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(tenant => tenant.Description)
            .HasMaxLength(2000);

        builder.Property(tenant => tenant.Slug)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(tenant => tenant.Slug)
            .IsUnique();

        builder.Property(tenant => tenant.Metadata);
    }
}
