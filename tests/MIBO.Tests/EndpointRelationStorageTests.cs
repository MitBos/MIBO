using MIBO.Domain.Models;
using MIBO.Infrastructure.Services;
using Xunit;

namespace MIBO.Tests;

public class EndpointRelationStorageTests
{
    [Fact]
    public async Task SaveAndLoad_RoundTripsRelation()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var storage = new FileEndpointRelationStorage(tempDir);

        var relation = new EndpointRelation
        {
            SourceSystemKey = "Source",
            SourceEndpointKey = "List",
            Targets =
            {
                new EndpointRelationTarget
                {
                    TargetSystemKey = "Target",
                    TargetEndpointKey = "Create",
                    MappingReference = "Mappings/list-to-create.json"
                }
            }
        };

        await storage.SaveAsync(relation);

        var all = await storage.ListAsync();
        Assert.Single(all);

        var loaded = await storage.LoadAsync("Source", "List");
        Assert.NotNull(loaded);
        Assert.Single(loaded!.Targets);
        Assert.Equal("Mappings/list-to-create.json", loaded.Targets[0].MappingReference);
    }
}
