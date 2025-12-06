// Summary: Minimal JSON mapping engine that projects configured source paths into a flat target object.
using System.Text.Json;
using MIBO.Application.Abstractions;
using MIBO.Domain.Models;

namespace MIBO.Infrastructure.Services;

public class BasicMappingEngine : IMappingEngine
{
    public string Map(string sourceJson, MappingConfig config)
    {
        using var document = JsonDocument.Parse(sourceJson);
        var root = document.RootElement;
        var target = new Dictionary<string, object?>();

        foreach (var mapping in config.Mappings)
        {
            var value = ResolveValue(root, mapping.SourceField);
            target[mapping.TargetField] = value;
        }

        return JsonSerializer.Serialize(target, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static object? ResolveValue(JsonElement element, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        JsonElement current = element;

        foreach (var segment in segments)
        {
            if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(segment, out var property))
            {
                current = property;
                continue;
            }

            return null;
        }

        return current.ValueKind switch
        {
            JsonValueKind.String => current.GetString(),
            JsonValueKind.Number => current.TryGetInt64(out var l) ? l : current.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => current.GetRawText()
        };
    }
}

