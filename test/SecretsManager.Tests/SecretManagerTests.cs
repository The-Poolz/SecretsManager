using Xunit;
using Amazon.SecretsManager;
using SecretsManager.Tests.Models;
using SecretsManager.Tests.Builders;

namespace SecretsManager.Tests;

public class SecretManagerTests : SecretManager
{
    private static string DeserializeExceptionMessage =>
        $"Could not deserialize the secret response to type {typeof(DBConnection)}.";

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

        var result = new SecretManager(client.Object).GetSecretValue<DBConnection>(SecretsManagerMockBuilder.SecretName);

        Assert.NotNull(result);
        Assert.Equal("secret connection", result.ConnectionString);
    }

    [Fact]
    public void GetSecretValue_SecretNotFound_ThrowsException()
    {
        var client = new SecretsManagerMockBuilder()
            .WithSecretString(string.Empty)
            .Build();

        Action testCode = () => new SecretManager(client.Object).GetSecretValue<DBConnection>(SecretsManagerMockBuilder.SecretName);

        var exception = Assert.Throws<InvalidOperationException>(testCode);
        Assert.Equal(DeserializeExceptionMessage, exception.Message);
    }
}