// Summary: JSON file-based storage for endpoint relations between API systems.
using System.Text.Json;
using System.Text.Json.Serialization;
using MIBO.Application.Abstractions;
using MIBO.Domain.Models;

namespace MIBO.Infrastructure.Services;

public class FileEndpointRelationStorage : IEndpointRelationStorage
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public FileEndpointRelationStorage(string directory)
    {
        _directory = directory;
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        Directory.CreateDirectory(_directory);
    }

    public async Task<List<EndpointRelation>> ListAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!Directory.Exists(_directory))
            {
                return new List<EndpointRelation>();
            }

            var relations = new List<EndpointRelation>();
            foreach (var file in Directory.EnumerateFiles(_directory, "*.json"))
            {
                await using var stream = File.OpenRead(file);
                var relation = await JsonSerializer.DeserializeAsync<EndpointRelation>(stream, _options, cancellationToken);
                if (relation is not null)
                {
                    relation.Targets ??= new List<EndpointRelationTarget>();
                    NormalizeTargets(relation);
                    relations.Add(relation);
                }
            }

            return relations
                .OrderBy(r => r.SourceSystemKey)
                .ThenBy(r => r.SourceEndpointKey)
                .ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<EndpointRelation?> LoadAsync(string sourceSystemKey, string sourceEndpointKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceSystemKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceEndpointKey);

        var path = GetPath(sourceSystemKey, sourceEndpointKey);
        if (!File.Exists(path))
        {
            return null;
        }

        await using var stream = File.OpenRead(path);
        var relation = await JsonSerializer.DeserializeAsync<EndpointRelation>(stream, _options, cancellationToken);
        if (relation is not null)
        {
            relation.Targets ??= new List<EndpointRelationTarget>();
            NormalizeTargets(relation);
        }

        return relation;
    }

    public async Task SaveAsync(EndpointRelation relation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.SourceSystemKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.SourceEndpointKey);

        relation.Targets ??= new List<EndpointRelationTarget>();
        NormalizeTargets(relation);

        var path = GetPath(relation.SourceSystemKey, relation.SourceEndpointKey);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, relation, _options, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteAsync(string sourceSystemKey, string sourceEndpointKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceSystemKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceEndpointKey);

        var path = GetPath(sourceSystemKey, sourceEndpointKey);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private string GetPath(string sourceSystemKey, string sourceEndpointKey)
    {
        var safeSystem = string.Join("_", sourceSystemKey.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        var safeEndpoint = string.Join("_", sourceEndpointKey.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(_directory, $"{safeSystem}__{safeEndpoint}.json");
    }

    private static void NormalizeTargets(EndpointRelation relation)
    {
        foreach (var target in relation.Targets)
        {
            target.Mappings ??= new List<FieldMapping>();
        }
    }
}
