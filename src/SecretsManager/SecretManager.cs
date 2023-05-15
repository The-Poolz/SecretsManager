using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using SecretsManager.Serialization;

namespace SecretsManager;

public class SecretManager
{
    protected readonly IAmazonSecretsManager client;

    public SecretManager(IAmazonSecretsManager? client = null)
    {
        this.client = client ?? CreateClient();
    }

    public virtual T GetSecretValue<T>(string secretName)
    {
        string secretResponse = GetSecretString(secretName);

        return JsonDeserializer.Deserialize<T>(secretResponse);
    }

    protected string GetSecretString(string secretName)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };

        var response = client.GetSecretValueAsync(request)
            .GetAwaiter()
            .GetResult();

        return response.SecretString;
    }

    protected static IAmazonSecretsManager CreateClient() =>
        new AmazonSecretsManagerClient(RegionProvider.GetRegionEndpoint());
}
