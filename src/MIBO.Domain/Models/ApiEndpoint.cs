// Summary: Describes an API endpoint, including hierarchy metadata and optional sample payload.
using System.ComponentModel.DataAnnotations;

namespace MIBO.Domain.Models;

public class ApiEndpoint
{
    [Required]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Endpoint key is required.")]
    public string Key { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "System key is required.")]
    public string SystemKey { get; set; } = string.Empty;

    [Required]
    [StringLength(256, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 256 characters.")]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(GET|POST|PUT|PATCH|DELETE)$", ErrorMessage = "HTTP method must be a valid verb.")]
    public string Method { get; set; } = "GET";

    [Required]
    [StringLength(256, MinimumLength = 1, ErrorMessage = "Path is required.")]
    public string Path { get; set; } = string.Empty;

    [StringLength(256, ErrorMessage = "Hierarchy key is too long.")]
    public string? HierarchyKey { get; set; }

    public string? SampleJson { get; set; }
}
