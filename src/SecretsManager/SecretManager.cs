using FluentValidation;
using Amazon.SecretsManager;
using SecretsManager.Validation;
using Amazon.SecretsManager.Model;

namespace SecretsManager;

public class SecretManager
{
    private readonly IAmazonSecretsManager client;

    public SecretManager(IAmazonSecretsManager? client = null)
    {
        this.client = client ?? CreateClient();
    }

    public virtual T TryGetSecretValue<T>(string secretName, T modelOfSecret)
    {
        string secretResponse = GetSecretString(secretName);

        var validator = new DeserializeValidator<T>(secretResponse);

        validator.ValidateAndThrow(modelOfSecret);

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

    private static IAmazonSecretsManager CreateClient() =>
        new AmazonSecretsManagerClient(RegionProvider.GetRegionEndpoint());
}
