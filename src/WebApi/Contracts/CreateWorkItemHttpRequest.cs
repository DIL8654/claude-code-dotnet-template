using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts;

public sealed class CreateWorkItemHttpRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }
}
