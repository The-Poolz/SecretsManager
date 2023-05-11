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
        var secretName = Environment.GetEnvironmentVariable("SECRET_NAME_OF_CONNECTION")
            ?? throw new ArgumentNullException("SECRET_NAME_OF_CONNECTION");

        var connection = await GetSecretValueAsync<DBConnection>(secretName, client ?? CreateClient());
        if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            throw new ArgumentNullException(nameof(connection.ConnectionString));

        return connection.ConnectionString;
    }

    public static async Task<T> GetSecretValueAsync<T>(string secretName, IAmazonSecretsManager client) where T : class
    {
        string secretResponse = await GetSecretAsync(secretName, client);

        T? secretValue = JsonConvert.DeserializeObject<T>(secretResponse);

        return secretValue ?? throw new ArgumentNullException(nameof(secretValue));
    }

    public static async Task<string> GetSecretAsync(string secretName, IAmazonSecretsManager client)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };

        GetSecretValueResponse response;
        try
        {
            response = await client.GetSecretValueAsync(request);
        }
        catch (Exception)
        {
            throw;
        }

        if (string.IsNullOrWhiteSpace(response.SecretString))
            throw new ArgumentNullException(nameof(response.SecretString));

        return response.SecretString;
    }

    public static IAmazonSecretsManager CreateClient() =>
        new AmazonSecretsManagerClient(
            RegionEndpoint.GetBySystemName(
                Environment.GetEnvironmentVariable("AWS_REGION")));

}
