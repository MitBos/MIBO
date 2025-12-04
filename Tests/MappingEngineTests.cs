using MIBO.Models;
using MIBO.Services;
using System.Text.Json;
using Xunit;

namespace MIBO.Tests;

public class MappingEngineTests
{
    [Fact]
    public void Map_ProjectsRequestedFields()
    {
        var engine = new BasicMappingEngine();
        var sourceJson = "{\"user\":{\"id\":42,\"name\":\"Ada\"}}";
        var config = new MappingConfig
        {
            Name = "test",
            Mappings =
            {
                new FieldMapping { SourceField = "user.id", TargetField = "UserId" },
                new FieldMapping { SourceField = "user.name", TargetField = "UserName" }
            }
        };

        var mapped = engine.Map(sourceJson, config);
        var document = JsonDocument.Parse(mapped);

        Assert.Equal(42, document.RootElement.GetProperty("UserId").GetInt32());
        Assert.Equal("Ada", document.RootElement.GetProperty("UserName").GetString());
    }
}
