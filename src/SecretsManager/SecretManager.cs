using Amazon;
using Newtonsoft.Json;
using Amazon.SecretsManager;
using SecretsManager.Models;
using Amazon.SecretsManager.Model;

namespace SecretsManager;

public class SecretManager
{
    private readonly IAmazonSecretsManager client;

    public SecretManager(IAmazonSecretsManager client)
    {
        this.client = client;
    }

    public virtual async Task<string> GetDbConnectionAsync()
    {
        var secretName = Environment.GetEnvironmentVariable("SECRET_NAME_OF_CONNECTION");
        if (string.IsNullOrWhiteSpace(secretName))
            throw new InvalidOperationException("The environment 'SECRET_NAME_OF_CONNECTION' cannot be null or empty.");

        var connection = await GetSecretValueAsync<DBConnection>(secretName);
        if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            throw new InvalidOperationException("The connection string cannot be null or empty.");

        return connection.ConnectionString;
    }

    public virtual async Task<T> GetSecretValueAsync<T>(string secretName) where T : class
    {
        string secretResponse = await GetSecretAsync(secretName);

        T? secretValue = JsonConvert.DeserializeObject<T>(secretResponse);

        return secretValue ?? throw new InvalidOperationException($"Could not deserialize the secret response to type {typeof(T)}.");
    }

    public virtual async Task<string> GetSecretAsync(string secretName)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };

        var response = await client.GetSecretValueAsync(request);

        if (string.IsNullOrWhiteSpace(response.SecretString))
            throw new InvalidOperationException("The secret string cannot be null or empty.");

        return response.SecretString;
    }

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
