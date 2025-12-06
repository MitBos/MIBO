// Summary: Represents a target endpoint selection in an integration, pointing at the API catalog.
using System.ComponentModel.DataAnnotations;

namespace MIBO.Domain.Models;

public class IntegrationTarget
{
    [Required(ErrorMessage = "Target system is required.")]
    [StringLength(128, ErrorMessage = "Target system key is too long.")]
    public string? TargetSystemKey { get; set; }

    [Required(ErrorMessage = "Target endpoint is required.")]
    [StringLength(128, ErrorMessage = "Target endpoint key is too long.")]
    public string? TargetEndpointKey { get; set; }

    [StringLength(256, ErrorMessage = "Mapping reference is too long.")]
    public string? MappingReference { get; set; }
}
