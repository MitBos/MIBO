// Summary: Maps REST endpoints for API systems, endpoints, sample payloads, and relations.
using MIBO.Application.Abstractions;
using MIBO.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MIBO.Web.Endpoints;

public static class ApiRegistryEndpoints
{
    public static IEndpointRouteBuilder MapApiRegistryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var systems = endpoints.MapGroup("/api/apisystems");

        systems.MapGet(string.Empty, async ([FromServices] IApiRegistryStorage storage) =>
        {
            var items = await storage.ListSystemsAsync();
            return Results.Ok(items);
        });

        systems.MapGet("{systemKey}", async (string systemKey, [FromServices] IApiRegistryStorage storage) =>
        {
            var system = await storage.LoadSystemAsync(systemKey);
            return system is null ? Results.NotFound() : Results.Ok(system);
        });

        systems.MapPost(string.Empty, async ([FromBody] ApiSystem system, [FromServices] IApiRegistryStorage storage) =>
        {
            if (string.IsNullOrWhiteSpace(system.Key) || string.IsNullOrWhiteSpace(system.DisplayName))
            {
                return Results.BadRequest(new { error = "System key and display name are required." });
            }

            await storage.SaveSystemAsync(system);
            return Results.Ok(system);
        });

        systems.MapGet("{systemKey}/endpoints", async (string systemKey, [FromServices] IApiRegistryStorage storage) =>
        {
            var endpoints = await storage.ListEndpointsAsync(systemKey);
            return Results.Ok(endpoints);
        });

        systems.MapGet("{systemKey}/endpoints/{endpointKey}", async (
            string systemKey,
            string endpointKey,
            [FromServices] IApiRegistryStorage storage) =>
        {
            var endpoint = await storage.LoadEndpointAsync(systemKey, endpointKey);
            return endpoint is null ? Results.NotFound() : Results.Ok(endpoint);
        });

        systems.MapPost("{systemKey}/endpoints", async (
            string systemKey,
            [FromBody] ApiEndpoint endpoint,
            [FromServices] IApiRegistryStorage storage) =>
        {
            if (string.IsNullOrWhiteSpace(endpoint.Key) || string.IsNullOrWhiteSpace(endpoint.DisplayName))
            {
                return Results.BadRequest(new { error = "Endpoint key and display name are required." });
            }

            endpoint.SystemKey = systemKey;
            await storage.SaveEndpointAsync(endpoint);
            return Results.Ok(endpoint);
        });

        return endpoints;
    }

    public static IEndpointRouteBuilder MapEndpointRelationApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/endpoint-relations");

        group.MapGet(string.Empty, async ([FromServices] IEndpointRelationStorage storage) =>
        {
            var relations = await storage.ListAsync();
            return Results.Ok(relations);
        });

        group.MapGet("{systemKey}/{endpointKey}", async (
            string systemKey,
            string endpointKey,
            [FromServices] IEndpointRelationStorage storage) =>
        {
            var relation = await storage.LoadAsync(systemKey, endpointKey);
            return relation is null ? Results.NotFound() : Results.Ok(relation);
        });

        group.MapPost(string.Empty, async (
            [FromBody] EndpointRelation relation,
            [FromServices] IEndpointRelationStorage storage) =>
        {
            if (string.IsNullOrWhiteSpace(relation.SourceSystemKey) ||
                string.IsNullOrWhiteSpace(relation.SourceEndpointKey))
            {
                return Results.BadRequest(new { error = "SourceSystemKey and SourceEndpointKey are required." });
            }

            relation.Targets ??= new List<EndpointRelationTarget>();
            await storage.SaveAsync(relation);
            return Results.Ok(relation);
        });

        group.MapDelete("{systemKey}/{endpointKey}", async (
            string systemKey,
            string endpointKey,
            [FromServices] IEndpointRelationStorage storage) =>
        {
            await storage.DeleteAsync(systemKey, endpointKey);
            return Results.NoContent();
        });

        return endpoints;
    }
}
