using Domain.WorkItems;

namespace Application.Abstractions;

public interface IWorkItemRepository
{
    public Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<IReadOnlyList<WorkItem>> ListAsync(int skip, int take, CancellationToken cancellationToken);

    public Task AddAsync(WorkItem workItem, CancellationToken cancellationToken);
}
