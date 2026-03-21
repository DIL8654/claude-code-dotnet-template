using Domain.WorkItems;

namespace Application.WorkItems;

public sealed record WorkItemDto(
    Guid Id,
    string Title,
    string? Description,
    WorkItemStatus Status,
    DateTimeOffset CreatedUtc,
    DateTimeOffset? LastModifiedUtc);
