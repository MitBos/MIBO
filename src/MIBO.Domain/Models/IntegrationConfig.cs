// Summary: Defines the integration configuration contract and validation rules for source/target calls and mappings.
using System;
using System.ComponentModel.DataAnnotations;

namespace MIBO.Domain.Models;

public class IntegrationConfig
{
    [Required(ErrorMessage = "Integration name is required.")]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 128 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(In|Out)$", ErrorMessage = "MethodDirection must be In or Out.")]
    public string MethodDirection { get; set; } = "In";

    [StringLength(256, ErrorMessage = "API key cannot exceed 256 characters.")]
    public string? ApiKey { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Rate limit must be zero or positive.")]
    public int? RateLimitPerMinute { get; set; }

    [StringLength(128, ErrorMessage = "System key is too long.")]
    public string? ApiSystemKey { get; set; }

    [StringLength(128, ErrorMessage = "Endpoint key is too long.")]
    public string? ApiEndpointKey { get; set; }

    // Manual URL for this method configuration (required until catalogs fully cover all endpoints).
    [Required(ErrorMessage = "URL is required for a method configuration.")]
    [Url(ErrorMessage = "URL must be a valid absolute URL.")]
    public string? SourceUrl { get; set; }

    // Manual HTTP method for this method configuration.
    [Required]
    [RegularExpression("^(GET|POST|PUT|PATCH|DELETE)$", ErrorMessage = "HTTP method must be a valid verb.")]
    public string? SourceMethod { get; set; } = "GET";

    // Optional schemas describing expected payloads for this method.
    public string? InputSchemaJson { get; set; }
    public string? OutputSchemaJson { get; set; }

    // Legacy/compat fields kept for backward compatibility but not used in the method-centric model.
    [Obsolete("Mappings now belong to endpoint relations (method IN -> method OUT).")]
    public List<FieldMapping> Mappings { get; set; } = new();
}
