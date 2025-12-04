using System.ComponentModel.DataAnnotations;
using MIBO.Models;

namespace MIBO.Services;

public class IntegrationEditorState
{
    public IntegrationConfig Config { get; private set; } = new();
    public ApiCallResult? ApiResult { get; private set; }
    public string? MappedJson { get; private set; }

    public void SetConfig(IntegrationConfig config)
    {
        Config = config;
        Config.Mappings ??= new List<FieldMapping>();
    }

    public void AddMapping()
    {
        Config.Mappings ??= new List<FieldMapping>();
        Config.Mappings.Add(new FieldMapping());
    }

    public void RemoveMapping(int index)
    {
        if (Config.Mappings is null) return;
        if (index < 0 || index >= Config.Mappings.Count) return;

        Config.Mappings.RemoveAt(index);
    }

    public void SetResult(RunIntegrationResponse? response)
    {
        ApiResult = response?.ApiResult;
        MappedJson = response?.MappedJson;
    }

    public bool Validate(out List<string> errors)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(Config);
        var isValid = Validator.TryValidateObject(Config, context, results, validateAllProperties: true);

        foreach (var mapping in Config.Mappings)
        {
            var mappingContext = new ValidationContext(mapping);
            isValid &= Validator.TryValidateObject(mapping, mappingContext, results, validateAllProperties: true);
        }

        if (Config.IntegrationType == "Sync" && string.IsNullOrWhiteSpace(Config.TargetUrl))
        {
            results.Add(new ValidationResult("Target URL is required for Sync integrations.", new[] { nameof(Config.TargetUrl) }));
            isValid = false;
        }

        errors = results.Select(r => r.ErrorMessage ?? "Validation error").ToList();
        return isValid;
    }
}
