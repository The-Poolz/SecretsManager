using Amazon;
using Newtonsoft.Json;
using Amazon.SecretsManager;
using SecretsManager.Models;
using Amazon.SecretsManager.Model;

namespace SecretsManager;

public static class SecretManager
{
    public static async Task<string> GetDbConnectionAsync(IAmazonSecretsManager? client = null)
    {
        var secretName = Environment.GetEnvironmentVariable("SECRET_NAME_OF_CONNECTION");
        if (string.IsNullOrWhiteSpace(secretName))
            throw new ArgumentNullException("secretName", "The environment 'SECRET_NAME_OF_CONNECTION' cannot be null or empty.");

        var connection = await GetSecretValueAsync<DBConnection>(secretName, client ?? CreateClient());
        if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            throw new ArgumentNullException(nameof(connection.ConnectionString) ,"The connection string cannot be null or empty.");

        return connection.ConnectionString;
    }

    public static async Task<T> GetSecretValueAsync<T>(string secretName, IAmazonSecretsManager client) where T : class
    {
        string secretResponse = await GetSecretAsync(secretName, client);

        T? secretValue = JsonConvert.DeserializeObject<T>(secretResponse);

        return secretValue ?? throw new ArgumentNullException(nameof(secretValue), $"Could not deserialize the secret response to type {typeof(T)}.");
    }

    public static async Task<string> GetSecretAsync(string secretName, IAmazonSecretsManager client)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };

        var response = await client.GetSecretValueAsync(request);

        if (string.IsNullOrWhiteSpace(response.SecretString))
            throw new ArgumentNullException(nameof(response.SecretString), "The secret string cannot be null or empty.");

        return response.SecretString;
    }

    public static IAmazonSecretsManager CreateClient() =>
        new AmazonSecretsManagerClient(
            RegionEndpoint.GetBySystemName(
                Environment.GetEnvironmentVariable("AWS_REGION")));
}
