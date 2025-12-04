using System.ComponentModel.DataAnnotations;

namespace MIBO.Models;

public class IntegrationConfig
{
    [Required(ErrorMessage = "Integration name is required.")]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 128 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Sync|Facade)$", ErrorMessage = "Integration type must be Sync or Facade.")]
    public string IntegrationType { get; set; } = "Sync";

    public bool FacadePassThrough { get; set; } = true;

    public bool ApiKeyRequired { get; set; }

    [StringLength(256, ErrorMessage = "API key cannot exceed 256 characters.")]
    public string? ApiKey { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Rate limit must be zero or positive.")]
    public int? RateLimitPerMinute { get; set; }

    [Required(ErrorMessage = "Source URL is required.")]
    [Url(ErrorMessage = "Source URL must be a valid absolute URL.")]
    public string SourceUrl { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(GET|POST|PUT|PATCH|DELETE)$", ErrorMessage = "Source method must be a valid HTTP verb.")]
    public string SourceMethod { get; set; } = "GET";

    [Url(ErrorMessage = "Target URL must be a valid absolute URL.")]
    public string? TargetUrl { get; set; }

    [Required]
    [RegularExpression("^(POST|PUT|PATCH)$", ErrorMessage = "Target method must be POST, PUT or PATCH.")]
    public string TargetMethod { get; set; } = "POST";

    [MinLength(0)]
    public List<FieldMapping> Mappings { get; set; } = new();
}
