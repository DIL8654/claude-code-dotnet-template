using Domain.WorkItems;

namespace Application.WorkItems;

public static class WorkItemMappings
{
    public static WorkItemDto ToDto(this WorkItem workItem) =>
        new(
            workItem.Id,
            workItem.Title,
            workItem.Description,
            workItem.Status,
            workItem.CreatedUtc,
            workItem.LastModifiedUtc);
}
