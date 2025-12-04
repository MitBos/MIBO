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
                    SourceUrl = "https://jsonplaceholder.typicode.com/posts/1",
                    SourceMethod = "GET",
                    TargetMethod = "POST",
                    IntegrationType = "Sync",
                    FacadePassThrough = true,
                    Mappings = new List<FieldMapping>
                    {
                        new() { SourceField = "userId", TargetField = "EmployeeId" },
                        new() { SourceField = "id",     TargetField = "ExternalId" },
                        new() { SourceField = "title",  TargetField = "Title" },
                        new() { SourceField = "body",   TargetField = "Description" }
                    }
                };
            }

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

            await storage.SaveAsync(config);
            return Results.Ok(config);
        });

        group.MapPost("run", async (
            [FromBody] IntegrationConfig config,
            [FromServices] IIntegrationConfigStorage storage,
            [FromServices] IApiCaller apiCaller,
            [FromServices] IMappingEngine mappingEngine) =>
        {
            if (string.IsNullOrWhiteSpace(config.Name))
            {
                return Results.BadRequest(new { error = "Name is required." });
            }

            await storage.SaveAsync(config);

            var request = new ApiCallRequest
            {
                Url = config.SourceUrl,
                Method = config.SourceMethod
            };

            var apiResult = await apiCaller.ExecuteAsync(request);

            string? mapped = null;

            if (apiResult.Success && !string.IsNullOrWhiteSpace(apiResult.ResponseBody))
            {
                if (!config.FacadePassThrough && config.Mappings.Any())
                {
                    var mappingConfig = new MappingConfig
                    {
                        Name = config.Name,
                        Mappings = config.Mappings
                    };

                    mapped = mappingEngine.Map(apiResult.ResponseBody!, mappingConfig);
                }
                else
                {
                    mapped = apiResult.ResponseBody;
                }
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
                IntegrationType = source.IntegrationType,
                FacadePassThrough = source.FacadePassThrough,
                ApiKeyRequired = source.ApiKeyRequired,
                ApiKey = source.ApiKey,
                RateLimitPerMinute = source.RateLimitPerMinute,
                SourceUrl = source.SourceUrl,
                SourceMethod = source.SourceMethod,
                TargetUrl = source.TargetUrl,
                TargetMethod = source.TargetMethod,
                Mappings = source.Mappings.Select(m => new FieldMapping
                {
                    SourceField = m.SourceField,
                    TargetField = m.TargetField
                }).ToList()
            };

            await storage.SaveAsync(copy);
            return Results.Ok(copy);
        });

        return endpoints;
    }
}
