using Amazon;
using Newtonsoft.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace SecretsManager;

public class SecretManager
{
    private readonly IAmazonSecretsManager client;

    public SecretManager(IAmazonSecretsManager client)
    {
        this.client = client;
    }

    public virtual T GetSecretValue<T>(string secretName) where T : class
    {
        string secretResponse = GetSecretString(secretName);

        T? secretValue = JsonConvert.DeserializeObject<T>(secretResponse);

        if (secretValue == null)
            throw new InvalidOperationException($"Could not deserialize the secret response to type {typeof(T)}.");

        return secretValue;
    }

    private string GetSecretString(string secretName)
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

    // TDO: Make new class RegionProvider
    public static IAmazonSecretsManager CreateClient(RegionEndpoint? region = null)
    {
        if (region == null)
        {
            var envRegion = Environment.GetEnvironmentVariable("AWS_REGION")
                ?? throw new InvalidOperationException("The environment 'AWS_REGION' cannot be null or empty.");
            region = RegionEndpoint.GetBySystemName(envRegion);
        }

        return new AmazonSecretsManagerClient(region);
    }
}
