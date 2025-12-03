namespace MIBO.Models;

public class IntegrationConfig
{
    public string Name { get; set; } = string.Empty;
    public string IntegrationType { get; set; } = "Sync";

    public bool FacadePassThrough { get; set; } = true;

    public bool ApiKeyRequired { get; set; }
    public string? ApiKey { get; set; }

    public int? RateLimitPerMinute { get; set; }

    public string SourceUrl { get; set; } = string.Empty;
    public string SourceMethod { get; set; } = "GET";

    public string? TargetUrl { get; set; }
    public string TargetMethod { get; set; } = "POST";

    public List<FieldMapping> Mappings { get; set; } = new();
}
