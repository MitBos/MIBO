// Summary: Request payload used to duplicate an existing integration configuration to a new name.
namespace MIBO.Domain.Models;

public class DuplicateIntegrationRequest
{
    public string SourceName { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
}
