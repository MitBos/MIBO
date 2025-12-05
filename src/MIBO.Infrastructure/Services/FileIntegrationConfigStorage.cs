using System.Text.Json;
using System.Text.Json.Serialization;
using MIBO.Application.Abstractions;
using MIBO.Domain.Models;

namespace MIBO.Infrastructure.Services;

public class FileIntegrationConfigStorage : IIntegrationConfigStorage
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public FileIntegrationConfigStorage(string directory)
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

    public async Task<List<string>> ListAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!Directory.Exists(_directory))
            {
                return new List<string>();
            }

            return Directory.EnumerateFiles(_directory, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => name is not null)
                .Select(name => name!)
                .OrderBy(name => name)
                .ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IntegrationConfig?> LoadAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var path = GetFilePath(name);
        if (!File.Exists(path))
        {
            return null;
        }

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<IntegrationConfig>(stream, _options, cancellationToken);
    }

    public async Task SaveAsync(IntegrationConfig config, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentException.ThrowIfNullOrWhiteSpace(config.Name);

        var path = GetFilePath(config.Name);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, config, _options, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var path = GetFilePath(name);
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

    private string GetFilePath(string name)
    {
        var safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(_directory, $"{safeName}.json");
    }
}
