// Summary: Maps REST endpoints for listing, loading, saving, running, deleting, and duplicating integrations.
using MIBO.Application.Abstractions;
using MIBO.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MIBO.Web.Endpoints;

public static class IntegrationApiEndpoints
{
    public static IEndpointRouteBuilder MapIntegrationApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/integrations");

        group.MapGet(string.Empty, async ([FromServices] IIntegrationConfigStorage storage) =>
        {
            var names = await storage.ListAsync();
            return Results.Ok(names);
        });

        group.MapGet("{name}", async (string name, [FromServices] IIntegrationConfigStorage storage) =>
        {
            var config = await storage.LoadAsync(name);
            if (config is null)
            {
                config = new IntegrationConfig
                {
                    Name = name,
                    MethodDirection = "In",
                    ApiSystemKey = "Microsoft",
                    ApiEndpointKey = "CustomersList",
                    SourceUrl = "https://jsonplaceholder.typicode.com/posts/1",
                    SourceMethod = "GET",
                    InputSchemaJson = "{ \"value\": [{ \"id\": \"string\" }] }"
                };
            }

            config.Mappings ??= new List<FieldMapping>();

            return Results.Ok(config);
        });

        group.MapPost("save", async (
            [FromBody] IntegrationConfig config,
            [FromServices] IIntegrationConfigStorage storage) =>
        {
            if (string.IsNullOrWhiteSpace(config.Name))
            {
                return Results.BadRequest(new { error = "Name is required." });
            }

            config.SourceMethod ??= "GET";
            config.Mappings ??= new List<FieldMapping>();
            await storage.SaveAsync(config);
            return Results.Ok(config);
        });

        group.MapPost("run", async (
            [FromBody] IntegrationConfig config,
            [FromServices] IIntegrationConfigStorage storage,
            [FromServices] IApiCaller apiCaller,
            [FromServices] IApiRegistryStorage apiRegistryStorage) =>
        {
            if (string.IsNullOrWhiteSpace(config.Name))
            {
                return Results.BadRequest(new { error = "Name is required." });
            }

            config.SourceMethod ??= "GET";
            config.Mappings ??= new List<FieldMapping>();
            await storage.SaveAsync(config);

            var request = await ResolveSourceRequestAsync(config, apiRegistryStorage);
            if (request is null)
            {
                return Results.BadRequest(new { error = "Source endpoint or URL is required." });
            }

            var apiResult = await apiCaller.ExecuteAsync(request);

            string? mapped = null;

            if (apiResult.Success && !string.IsNullOrWhiteSpace(apiResult.ResponseBody))
            {
                mapped = apiResult.ResponseBody;
            }

            var response = new RunIntegrationResponse
            {
                ApiResult = apiResult,
                MappedJson = mapped
            };

            return Results.Ok(response);
        });

        // DELETE /api/integrations/{name} -> delete config
        group.MapDelete("{name}", async (string name, [FromServices] IIntegrationConfigStorage storage) =>
        {
            await storage.DeleteAsync(name);
            return Results.NoContent();
        });

        // POST /api/integrations/duplicate -> duplicate config
        group.MapPost("duplicate", async (
            [FromBody] DuplicateIntegrationRequest request,
            [FromServices] IIntegrationConfigStorage storage) =>
        {
            if (string.IsNullOrWhiteSpace(request.SourceName) ||
                string.IsNullOrWhiteSpace(request.TargetName))
            {
                return Results.BadRequest(new { error = "SourceName and TargetName are required." });
            }

            var source = await storage.LoadAsync(request.SourceName);
            if (source is null)
            {
                return Results.NotFound(new { error = $"Integration '{request.SourceName}' not found." });
            }

            var copy = new IntegrationConfig
            {
                Name = request.TargetName,
                ApiKey = source.ApiKey,
                RateLimitPerMinute = source.RateLimitPerMinute,
                MethodDirection = source.MethodDirection,
                ApiSystemKey = source.ApiSystemKey,
                ApiEndpointKey = source.ApiEndpointKey,
                SourceUrl = source.SourceUrl,
                SourceMethod = source.SourceMethod ?? "GET",
                InputSchemaJson = source.InputSchemaJson,
                OutputSchemaJson = source.OutputSchemaJson,
                Mappings = new List<FieldMapping>()
            };

            await storage.SaveAsync(copy);
            return Results.Ok(copy);
        });

        return endpoints;
    }

    private static async Task<ApiCallRequest?> ResolveSourceRequestAsync(
        IntegrationConfig config,
        IApiRegistryStorage apiRegistryStorage)
    {
        var systemKey = config.ApiSystemKey;
        var endpointKey = config.ApiEndpointKey;

        if (!string.IsNullOrWhiteSpace(systemKey) &&
            !string.IsNullOrWhiteSpace(endpointKey))
        {
            var endpoint = await apiRegistryStorage.LoadEndpointAsync(systemKey!, endpointKey!);
            if (endpoint is not null)
            {
                return new ApiCallRequest
                {
                    Url = endpoint.Path,
                    Method = endpoint.Method
                };
            }
        }

        if (!string.IsNullOrWhiteSpace(config.SourceUrl))
        {
            return new ApiCallRequest
            {
                Url = config.SourceUrl,
                Method = string.IsNullOrWhiteSpace(config.SourceMethod) ? "GET" : config.SourceMethod!
            };
        }

        return null;
    }
}
