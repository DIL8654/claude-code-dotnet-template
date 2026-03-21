namespace Application.WorkItems;

public sealed record CreateWorkItemRequest(string Title, string? Description);
