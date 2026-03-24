using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace WebApi.Contracts;

public sealed class CreateTenantHttpRequest
{
    [Required]
    [MaxLength(200)]
    public string TenantName { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(
        @"^[a-z0-9]([a-z0-9\-]{0,98}[a-z0-9])?$",
        ErrorMessage = "Slug must be 1–100 characters, lowercase alphanumeric and hyphens only, and must not start or end with a hyphen.")]
    public string Slug { get; init; } = string.Empty;

    public JsonElement? Metadata { get; init; }
}
