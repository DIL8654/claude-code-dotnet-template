using Application.Abstractions;
using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class WorkItemRepository(AppDbContext dbContext) : IWorkItemRepository
{
    public Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.WorkItems.SingleOrDefaultAsync(workItem => workItem.Id == id, cancellationToken);

    public async Task<IReadOnlyList<WorkItem>> ListAsync(int skip, int take, CancellationToken cancellationToken) =>
        await dbContext.WorkItems
            .AsNoTracking()
            .OrderByDescending(workItem => workItem.CreatedUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task AddAsync(WorkItem workItem, CancellationToken cancellationToken) =>
        dbContext.WorkItems.AddAsync(workItem, cancellationToken).AsTask();
}
