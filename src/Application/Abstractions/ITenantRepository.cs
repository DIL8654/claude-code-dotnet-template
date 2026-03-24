using Domain.Tenants;

namespace Application.Abstractions;

public interface ITenantRepository
{
    public Task AddAsync(Tenant tenant, CancellationToken cancellationToken);
}
