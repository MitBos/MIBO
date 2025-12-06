// Summary: JSON file-based storage for API systems and endpoints with concurrency protection.
using System.Text.Json;
using System.Text.Json.Serialization;
using MIBO.Application.Abstractions;
using MIBO.Domain.Models;

namespace MIBO.Infrastructure.Services;

public class FileApiRegistryStorage : IApiRegistryStorage
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public FileApiRegistryStorage(string directory)
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

    public async Task<List<ApiSystem>> ListSystemsAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!Directory.Exists(_directory))
            {
                return new List<ApiSystem>();
            }

            var systems = new List<ApiSystem>();
            foreach (var file in Directory.EnumerateFiles(_directory, "*.json"))
            {
                await using var stream = File.OpenRead(file);
                var system = await JsonSerializer.DeserializeAsync<ApiSystem>(stream, _options, cancellationToken);
                if (system is not null)
                {
                    NormalizeSystem(system);
                    systems.Add(system);
                }
            }

            return systems.OrderBy(s => s.DisplayName).ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<ApiSystem?> LoadSystemAsync(string systemKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(systemKey);

        var path = GetSystemPath(systemKey);
        if (!File.Exists(path))
        {
            return null;
        }

        await using var stream = File.OpenRead(path);
        var system = await JsonSerializer.DeserializeAsync<ApiSystem>(stream, _options, cancellationToken);
        if (system is not null)
        {
            NormalizeSystem(system);
        }

        return system;
    }

    public async Task SaveSystemAsync(ApiSystem system, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(system);
        ArgumentException.ThrowIfNullOrWhiteSpace(system.Key);
        ArgumentException.ThrowIfNullOrWhiteSpace(system.DisplayName);

        NormalizeSystem(system);

        var path = GetSystemPath(system.Key);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, system, _options, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteSystemAsync(string systemKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(systemKey);

        var path = GetSystemPath(systemKey);
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

    public async Task<List<ApiEndpoint>> ListEndpointsAsync(string systemKey, CancellationToken cancellationToken = default)
    {
        var system = await LoadSystemAsync(systemKey, cancellationToken);
        return system?.Endpoints
            .OrderBy(e => e.HierarchyKey ?? e.DisplayName)
            .ToList() ?? new List<ApiEndpoint>();
    }

    public async Task<ApiEndpoint?> LoadEndpointAsync(string systemKey, string endpointKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(systemKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointKey);

        var system = await LoadSystemAsync(systemKey, cancellationToken);
        return system?.Endpoints.FirstOrDefault(e => string.Equals(e.Key, endpointKey, StringComparison.OrdinalIgnoreCase));
    }

    public async Task SaveEndpointAsync(ApiEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint.SystemKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint.Key);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint.DisplayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint.Method);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint.Path);

        var system = await LoadSystemAsync(endpoint.SystemKey, cancellationToken) ?? new ApiSystem
        {
            Key = endpoint.SystemKey,
            DisplayName = endpoint.SystemKey,
            Endpoints = new List<ApiEndpoint>()
        };

        NormalizeSystem(system);
        endpoint.SystemKey = system.Key;

        var existing = system.Endpoints.FindIndex(e => string.Equals(e.Key, endpoint.Key, StringComparison.OrdinalIgnoreCase));
        if (existing >= 0)
        {
            system.Endpoints[existing] = endpoint;
        }
        else
        {
            system.Endpoints.Add(endpoint);
        }

        await SaveSystemAsync(system, cancellationToken);
    }

    public async Task DeleteEndpointAsync(string systemKey, string endpointKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(systemKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointKey);

        var system = await LoadSystemAsync(systemKey, cancellationToken);
        if (system is null)
        {
            return;
        }

        system.Endpoints.RemoveAll(e => string.Equals(e.Key, endpointKey, StringComparison.OrdinalIgnoreCase));
        await SaveSystemAsync(system, cancellationToken);
    }

    private string GetSystemPath(string systemKey)
    {
        var safeName = string.Join("_", systemKey.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(_directory, $"{safeName}.json");
    }

    private static void NormalizeSystem(ApiSystem system)
    {
        system.Endpoints ??= new List<ApiEndpoint>();
        foreach (var endpoint in system.Endpoints)
        {
            endpoint.SystemKey = string.IsNullOrWhiteSpace(endpoint.SystemKey) ? system.Key : endpoint.SystemKey;
        }
    }
}
