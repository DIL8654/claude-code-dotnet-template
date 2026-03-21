using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts;

public sealed record CreateWorkItemHttpRequest(
    [property: Required, MaxLength(200)] string Title,
    [property: MaxLength(2000)] string? Description);
