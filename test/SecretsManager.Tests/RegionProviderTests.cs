using Xunit;
using Amazon;

namespace SecretsManager.Tests;

public class RegionProviderTests
{
    [Fact]
    public void DefaultRegion_USEast1()
    {
        Environment.SetEnvironmentVariable("AWS_REGION", "");

        var result = RegionProvider.DefaultRegion;

        Assert.Equal(RegionEndpoint.USEast1, result);
    }

    [Fact]
    public void GetRegionEndpoint_WithoutEnvironment_DefaultRegion()
    {
        Environment.SetEnvironmentVariable("AWS_REGION", "");

        var result = RegionProvider.GetRegionEndpoint();

        Assert.Equal(RegionEndpoint.USEast1, result);
    }

    [Fact]
    public void GetRegionEndpoint_WithEnvironment_RegionWhoSetInEnv()
    {
        Environment.SetEnvironmentVariable("AWS_REGION", RegionEndpoint.USEast2.SystemName);

        var result = RegionProvider.GetRegionEndpoint();

        Assert.Equal(RegionEndpoint.USEast2, result);
    }
}
