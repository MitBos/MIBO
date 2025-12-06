// Summary: Represents a mapping between a source JSON path and a target JSON path with validation.
using System.ComponentModel.DataAnnotations;

namespace MIBO.Domain.Models;

public class FieldMapping
{
    [Required(ErrorMessage = "Source field is required.")]
    [StringLength(128, ErrorMessage = "Source field is too long.")]
    public string SourceField { get; set; } = string.Empty;

    [Required(ErrorMessage = "Target field is required.")]
    [StringLength(128, ErrorMessage = "Target field is too long.")]
    public string TargetField { get; set; } = string.Empty;
}

