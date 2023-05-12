using Xunit;
using Amazon;
using SecretsManager.Tests.Models;
using SecretsManager.Tests.Builders;

namespace SecretsManager.Tests;

public class SecretManagerTests
{
    private const string DeserializeExceptionMessage =
        "Could not deserialize the secret response to type SecretsManager.Tests.Models.DBConnection.";
    private const string EnvironmentNotSetExceptionMessage =
        $"The environment 'AWS_REGION' cannot be null or empty.";

    [Fact]
    public void CreateClient_SetFromEnvironment_ReturnsSecretsManagerClient()
    {
        var expectedRegionEndpoint = RegionEndpoint.AFSouth1;
        Environment.SetEnvironmentVariable("AWS_REGION", expectedRegionEndpoint.SystemName);

        var result = SecretManager.CreateClient();

        Assert.NotNull(result);
        Assert.Equal(expectedRegionEndpoint, result.Config.RegionEndpoint);
    }

    [Fact]
    public void CreateClient_SetFromEnvironment_ThrowsException()
    {
        Environment.SetEnvironmentVariable("AWS_REGION", "");

        Action testCode = () => SecretManager.CreateClient();

        var exception = Assert.Throws<InvalidOperationException>(testCode);
        Assert.Equal(EnvironmentNotSetExceptionMessage, exception.Message);
    }

    [Fact]
    public void CreateClient_SetFromParameter_ReturnsSecretsManagerClient()
    {
        var expectedRegionEndpoint = RegionEndpoint.AFSouth1;

        var result = SecretManager.CreateClient(expectedRegionEndpoint);

        Assert.NotNull(result);
        Assert.Equal(expectedRegionEndpoint, result.Config.RegionEndpoint);
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