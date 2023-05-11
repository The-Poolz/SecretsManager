using Moq;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace SecretsManager.Tests.Builders;

public class SecretsManagerMockBuilder
{
    private readonly GetSecretValueResponse secretValueResponse;
    private bool throwException;

    public SecretsManagerMockBuilder()
    {
        secretValueResponse = new GetSecretValueResponse
        {
            Name = "ConnectionToDB",
            VersionId = "01234567890123456789012345678901",
            SecretString = "{ \"connectionString\" : \"secret connection\" }"
        };
        throwException = false;
    }

    public SecretsManagerMockBuilder WithSecretName(string name)
    {
        secretValueResponse.Name = name;
        return this;
    }

    public SecretsManagerMockBuilder WithSecretString(string secretString)
    {
        secretValueResponse.SecretString = secretString;
        return this;
    }

    public SecretsManagerMockBuilder WithException()
    {
        throwException = true;
        return this;
    }

    public Mock<IAmazonSecretsManager> Build()
    {
        var secretsManager = new Mock<IAmazonSecretsManager>(MockBehavior.Strict);

        if (throwException)
        {
            secretsManager.Setup(i => i.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonSecretsManagerException("This should not be called"));
        }
        else
        {
            secretsManager.Setup(x => x.GetSecretValueAsync(
                It.IsAny<GetSecretValueRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetSecretValueRequest request, CancellationToken token) =>
            {
                if (request.SecretId == secretValueResponse.Name)
                    return secretValueResponse;

                return new GetSecretValueResponse();
            });
        }

        return secretsManager;
    }

}
