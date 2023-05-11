using Amazon;
using Newtonsoft.Json;
using Amazon.SecretsManager;
using Helpers.Secrets.Models;
using Helpers.Secrets.Exceptions;
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
            throw new Exception(ExceptionsMessages.SecretWithoutData(secretName));

        return connection.ConnectionString;
    }

    public static async Task<T> GetSecretValueAsync<T>(string secretName, IAmazonSecretsManager client) where T : class
    {
        string? secretString = await GetSecretAsync(secretName, client).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(secretString))
            throw new Exception(ExceptionsMessages.SecretNotFound());

        T? secretValue = JsonConvert.DeserializeObject<T>(secretString);
        if (secretValue == null)
            throw new Exception(ExceptionsMessages.DeserializeSecretError<T>());

        return secretValue;
    }

    public static async Task<string?> GetSecretAsync(string secretName, IAmazonSecretsManager client)
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

        if (response != null)
            return response.SecretString;

        return null;
    }

    public static IAmazonSecretsManager CreateClient() =>
        new AmazonSecretsManagerClient(
            RegionEndpoint.GetBySystemName(
                Environment.GetEnvironmentVariable("AWS_REGION")));
}
