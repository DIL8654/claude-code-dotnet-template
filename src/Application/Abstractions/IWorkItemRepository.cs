using Domain.WorkItems;

namespace Application.Abstractions;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<WorkItem>> ListAsync(int skip, int take, CancellationToken cancellationToken);

    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken);
}
