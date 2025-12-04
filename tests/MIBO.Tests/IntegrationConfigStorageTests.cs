using MIBO.Domain.Models;
using MIBO.Infrastructure.Services;
using Xunit;

namespace MIBO.Tests;

public class IntegrationConfigStorageTests
{
    [Fact]
    public async Task SaveAndLoad_RoundTripsConfig()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var storage = new FileIntegrationConfigStorage(tempDir);
        var config = new IntegrationConfig
        {
            Name = "Sample",
            SourceUrl = "https://example.com/source",
            TargetUrl = "https://example.com/target",
            IntegrationType = "Sync",
            Mappings =
            {
                new FieldMapping { SourceField = "id", TargetField = "Id" }
            }
        };

        await storage.SaveAsync(config);
        var names = await storage.ListAsync();
        Assert.Contains("Sample", names);

        var loaded = await storage.LoadAsync("Sample");
        Assert.NotNull(loaded);
        Assert.Equal(config.SourceUrl, loaded!.SourceUrl);
        Assert.Equal(config.Mappings.Count, loaded.Mappings.Count);
    }
}
