using Amazon;

namespace SecretsManager;

public static class RegionProvider
{
    public static RegionEndpoint DefaultRegion { get; set; } = RegionEndpoint.USEast1;

    public static RegionEndpoint GetRegionEndpoint()
    {
        var envRegion = Environment.GetEnvironmentVariable("AWS_REGION");
        if (!string.IsNullOrWhiteSpace(envRegion))
        {
            return RegionEndpoint.GetBySystemName(envRegion);
        }
        return DefaultRegion;
    }
}
