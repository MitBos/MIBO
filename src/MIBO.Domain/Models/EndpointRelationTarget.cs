// Summary: Describes a target endpoint within a relation and its mapping reference.
using System.ComponentModel.DataAnnotations;

namespace MIBO.Domain.Models;

public class EndpointRelationTarget
{
    [Required]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "Target system key is required.")]
    public string TargetSystemKey { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Target endpoint key is required.")]
    public string TargetEndpointKey { get; set; } = string.Empty;

    [StringLength(256, ErrorMessage = "Mapping reference is too long.")]
    public string? MappingReference { get; set; }

    // Preferred: inline mappings between method IN payload and method OUT payload.
    public List<FieldMapping> Mappings { get; set; } = new();
}
