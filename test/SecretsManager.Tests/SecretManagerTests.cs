using Xunit;
using Amazon.SecretsManager;
using SecretsManager.Tests.Builders;

namespace SecretsManager.Tests;

public class SecretManagerTests : SecretManager
{
    [Fact]
    public void Ctor_ClientNotPassed_ExcpectedClient()
    {
        var result = new SecretManager();

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateClient_ExcpectedClient()
    {
        var result = CreateClient();

        Assert.NotNull(result);
        Assert.IsType<AmazonSecretsManagerClient>(result);
    }

    [Fact]
    public void GetSecretValue_ValidSecret_ReturnsDeserializedSecret()
    {
        var client = new SecretsManagerMockBuilder()
            .Build();

        var result = new SecretManager(client.Object).GetSecretValue(SecretsManagerMockBuilder.SecretName, "connectionString");

        Assert.NotNull(result);
        Assert.Equal("secret connection", result);
    }

    [Fact]
    public void GetSecretValue_SecretNotFound_ThrowsException()
    {
        var client = new SecretsManagerMockBuilder()
            .Build();

        Action testCode = () => new SecretManager(client.Object).GetSecretValue(SecretsManagerMockBuilder.SecretName, "ConnectionString");

        var exception = Assert.Throws<KeyNotFoundException>(testCode);
        Assert.Equal($"The specified secret key 'ConnectionString' does not exist.", exception.Message);
    }
}