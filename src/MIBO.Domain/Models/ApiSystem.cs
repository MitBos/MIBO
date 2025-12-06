// Summary: Represents an API system entry and its declared endpoints.
using System.ComponentModel.DataAnnotations;

namespace MIBO.Domain.Models;

public class ApiSystem
{
    [Required]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "System key must be between 2 and 128 characters.")]
    public string Key { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 128 characters.")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(512, ErrorMessage = "Description is too long.")]
    public string? Description { get; set; }

    public List<ApiEndpoint> Endpoints { get; set; } = new();
}
