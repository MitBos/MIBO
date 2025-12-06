// Summary: Holds the current integration config, validation helpers, and run results for the edit UI.
using System.ComponentModel.DataAnnotations;
using MIBO.Domain.Models;

namespace MIBO.Web.Services;

public class IntegrationEditorState
{
    public IntegrationConfig Config { get; private set; } = new();
    public ApiCallResult? ApiResult { get; private set; }
    public string? MappedJson { get; private set; }

    public void SetConfig(IntegrationConfig config)
    {
        Config = config;
        Config.Mappings ??= new List<FieldMapping>();
        Config.SourceMethod ??= "GET";
        Config.SourceUrl ??= string.Empty;
        if (string.IsNullOrWhiteSpace(Config.MethodDirection))
        {
            Config.MethodDirection = "In";
        }
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

        var hasCatalogBinding = !string.IsNullOrWhiteSpace(Config.ApiSystemKey) &&
                                !string.IsNullOrWhiteSpace(Config.ApiEndpointKey);
        var hasManualBinding = !string.IsNullOrWhiteSpace(Config.SourceUrl) &&
                               !string.IsNullOrWhiteSpace(Config.SourceMethod);

        if (!hasCatalogBinding && !hasManualBinding)
        {
            results.Add(new ValidationResult("Select an API endpoint or provide URL + method.", new[] { nameof(Config.ApiSystemKey), nameof(Config.SourceUrl) }));
            isValid = false;
        }

        errors = results.Select(r => r.ErrorMessage ?? "Validation error").ToList();
        return isValid;
    }
}
