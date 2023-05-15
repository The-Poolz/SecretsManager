using Moq;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace SecretsManager.Tests.Builders;

public class SecretsManagerMockBuilder
{
    public static string SecretString => "{ \"connectionString\" : \"secret connection\" }";
    public static string SecretName => "ConnectionToDB";

    private readonly GetSecretValueResponse secretValueResponse;

    public SecretsManagerMockBuilder()
    {
        secretValueResponse = new GetSecretValueResponse
        {
            Name = "ConnectionToDB",
            VersionId = "01234567890123456789012345678901",
            SecretString = SecretString
        };
    }

    public Mock<IAmazonSecretsManager> Build()
    {
        var secretsManager = new Mock<IAmazonSecretsManager>(MockBehavior.Strict);

        secretsManager.Setup(x => x.GetSecretValueAsync(
            It.IsAny<GetSecretValueRequest>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync((GetSecretValueRequest request, CancellationToken token) =>
        {
            if (request.SecretId == secretValueResponse.Name)
                return secretValueResponse;

            return new GetSecretValueResponse();
        });

        return secretsManager;
    }
}
