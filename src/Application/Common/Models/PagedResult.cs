namespace Application.Common.Models;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Skip, int Take);
