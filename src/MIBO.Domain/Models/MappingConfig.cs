namespace MIBO.Domain.Models;

public class MappingConfig
{
    public string Name { get; set; } = string.Empty;
    public List<FieldMapping> Mappings { get; set; } = new();
}
