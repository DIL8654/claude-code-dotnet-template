using Domain.Abstractions;

namespace Domain.WorkItems;

public sealed class WorkItem : AuditableEntity
{
    private WorkItem()
    {
    }

    private WorkItem(string title, string? description, DateTimeOffset createdUtc)
    {
        Title = title;
        Description = description;
        CreatedUtc = createdUtc;
        Status = WorkItemStatus.Active;
    }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public WorkItemStatus Status { get; private set; } = WorkItemStatus.Draft;

    public byte[] RowVersion { get; private set; } = [];

    public static WorkItem Create(string title, string? description, DateTimeOffset createdUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        return new WorkItem(title.Trim(), description?.Trim(), createdUtc);
    }

    public void Rename(string title, DateTimeOffset updatedUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        Title = title.Trim();
        Touch(updatedUtc);
    }

    public void UpdateDescription(string? description, DateTimeOffset updatedUtc)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Touch(updatedUtc);
    }

    public void Complete(DateTimeOffset updatedUtc)
    {
        Status = WorkItemStatus.Completed;
        Touch(updatedUtc);
    }
}
