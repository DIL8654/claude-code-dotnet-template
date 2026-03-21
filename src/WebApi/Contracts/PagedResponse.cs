namespace WebApi.Contracts;

public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Skip, int Take);
