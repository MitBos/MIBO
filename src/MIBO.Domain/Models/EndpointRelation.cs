// Summary: Links a source endpoint to one or more target endpoints and mapping references.
using System.ComponentModel.DataAnnotations;

namespace MIBO.Domain.Models;

public class EndpointRelation
{
    [Required]
    [StringLength(128, MinimumLength = 2, ErrorMessage = "Source system key is required.")]
    public string SourceSystemKey { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Source endpoint key is required.")]
    public string SourceEndpointKey { get; set; } = string.Empty;

    [MinLength(0)]
    public List<EndpointRelationTarget> Targets { get; set; } = new();
}
