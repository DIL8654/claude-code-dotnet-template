using Application.Abstractions;
using Application.Common.Exceptions;
using Application.Common.Models;
using Domain.WorkItems;

namespace Application.WorkItems;

public sealed class WorkItemService(
    IWorkItemRepository workItemRepository,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<WorkItemDto> CreateAsync(CreateWorkItemRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        WorkItem workItem = WorkItem.Create(request.Title, request.Description, clock.UtcNow);

        await workItemRepository.AddAsync(workItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workItem.ToDto();
    }

    public async Task<WorkItemDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await workItemRepository.GetByIdAsync(id, cancellationToken);
        return workItem is null
            ? throw new NotFoundException($"Work item '{id}' was not found.")
            : workItem.ToDto();
    }

    public async Task<PagedResult<WorkItemDto>> ListAsync(int skip, int take, CancellationToken cancellationToken)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), "Skip must be zero or greater.");
        }

        if (take <= 0 || take > 200)
        {
            throw new ArgumentOutOfRangeException(nameof(take), "Take must be between 1 and 200.");
        }

        IReadOnlyList<WorkItemDto> items = (await workItemRepository.ListAsync(skip, take, cancellationToken))
            .Select(workItem => workItem.ToDto())
            .ToArray();

        return new PagedResult<WorkItemDto>(items, skip, take);
    }
}
