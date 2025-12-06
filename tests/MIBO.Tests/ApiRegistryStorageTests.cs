using MIBO.Domain.Models;
using MIBO.Infrastructure.Services;
using Xunit;

namespace MIBO.Tests;

public class ApiRegistryStorageTests
{
    [Fact]
    public async Task SaveEndpoint_RoundTripsSystemAndEndpoint()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var storage = new FileApiRegistryStorage(tempDir);

        var endpoint = new ApiEndpoint
        {
            SystemKey = "Demo",
            Key = "GetCustomer",
            DisplayName = "Get customer",
            Method = "GET",
            Path = "/customers/{id}",
            HierarchyKey = "Customers/Get",
            SampleJson = "{ \"id\": 1 }"
        };

        await storage.SaveEndpointAsync(endpoint);

        var systems = await storage.ListSystemsAsync();
        Assert.Single(systems);
        Assert.Equal("Demo", systems[0].Key);

        var loadedEndpoint = await storage.LoadEndpointAsync("Demo", "GetCustomer");
        Assert.NotNull(loadedEndpoint);
        Assert.Equal(endpoint.SampleJson, loadedEndpoint!.SampleJson);
    }
}
