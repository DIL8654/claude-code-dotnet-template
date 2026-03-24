using Application.Abstractions;
using Domain.Tenants;

namespace Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(AppDbContext dbContext) : ITenantRepository
{
    public Task AddAsync(Tenant tenant, CancellationToken cancellationToken) =>
        dbContext.Tenants.AddAsync(tenant, cancellationToken).AsTask();
}
