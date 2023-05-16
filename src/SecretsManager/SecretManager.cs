using Newtonsoft.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace SecretsManager;

public class SecretManager
{
    protected readonly IAmazonSecretsManager client;

    public SecretManager(IAmazonSecretsManager? client = null)
    {
        this.client = client ?? CreateClient();
    }

    public virtual string GetSecretValue(string secretId, string secretKey)
    {
        string secretString = GetSecretString(secretId);

        var secretResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretString)!;

        if (!secretResponse.ContainsKey(secretKey))
        {
            throw new KeyNotFoundException($"The specified secret key '{secretKey}' does not exist.");
        }

        return secretResponse[secretKey];
    }

    protected string GetSecretString(string secretId)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretId,
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
