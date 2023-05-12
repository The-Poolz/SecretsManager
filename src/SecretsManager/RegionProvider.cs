using Amazon;

namespace SecretsManager;

public class RegionProvider
{
    public RegionEndpoint Region { get; set; }

    public RegionProvider()
    {
        Region = RegionEndpoint.EUCentral1;
    }
}
