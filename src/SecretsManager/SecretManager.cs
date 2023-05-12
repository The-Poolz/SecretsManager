using Amazon;
using FluentValidation;
using Amazon.SecretsManager;
using SecretsManager.Validation;
using Amazon.SecretsManager.Model;

namespace SecretsManager;

public class SecretManager
{
    private readonly IAmazonSecretsManager client;

    public SecretManager(IAmazonSecretsManager client)
    {
        this.client = client;
    }

    public virtual T TryGetSecretValue<T>(string secretName, T secret)
    {
        string secretResponse = GetSecretString(secretName);

        var validator = new DeserializeValidator<T>(secretResponse);

        validator.ValidateAndThrow(secret);

        return validator.DeserializedObject;
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
