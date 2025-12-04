using MIBO.Models;
using MIBO.Services;
using Xunit;

namespace MIBO.Tests;

public class IntegrationEditorStateTests
{
    [Fact]
    public void Validate_RequiresTargetUrlForSync()
    {
        var state = new IntegrationEditorState();
        state.SetConfig(new IntegrationConfig
        {
            Name = "demo",
            IntegrationType = "Sync",
            SourceUrl = "https://example.com/source"
        });

        var isValid = state.Validate(out var errors);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.Contains("Target URL"));
    }
}
