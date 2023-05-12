using Xunit;
using Amazon;
using Amazon.SecretsManager;
using SecretsManager.Models;
using SecretsManager.Tests.Builders;

namespace SecretsManager.Tests;

public class SecretManagerTests
{
    private const string NullSecretStringExceptionMessage =
        "The secret string cannot be null or empty.";
    private const string NullConnectionStringExceptionMessage =
        "The connection string cannot be null or empty.";
    private static string EnvironmentNotSetExceptionMessage(string envName) =>
        $"The environment '{envName}' cannot be null or empty.";

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
        Assert.Equal(EnvironmentNotSetExceptionMessage("AWS_REGION"), exception.Message);
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
    public async Task GetSecretAsync_ValidSecret_ReturnsSecretString()
    {
        var client = new SecretsManagerMockBuilder()
            .Build();

        var result = await new SecretManager(client.Object).GetSecretAsync(SecretsManagerMockBuilder.SecretName);

        Assert.Equal(SecretsManagerMockBuilder.SecretString, result);
    }

    [Fact]
    public async Task GetSecretAsync_SecretNotFound_ThrowsException()
    {
        var client = new SecretsManagerMockBuilder()
            .Build();

        Func<Task> testCode = () => new SecretManager(client.Object).GetSecretAsync("nonExistentSecret");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(testCode);
        Assert.Equal(NullSecretStringExceptionMessage, exception.Message);
    }

    [Fact]
    public async Task GetSecretAsync_ExceptionThrown_ThrowsException()
    {
        var client = new SecretsManagerMockBuilder()
            .WithException()
            .Build();

        Func<Task> testCode = () => new SecretManager(client.Object).GetSecretAsync("secret");

        var exception = await Assert.ThrowsAsync<AmazonSecretsManagerException>(testCode);
        Assert.Equal("This should not be called", exception.Message);
    }

    [Fact]
    public async Task GetSecretValueAsync_ValidSecret_ReturnsDeserializedSecret()
    {
        var client = new SecretsManagerMockBuilder()
            .Build();

        var result = await new SecretManager(client.Object).GetSecretValueAsync<DBConnection>(SecretsManagerMockBuilder.SecretName);

        Assert.NotNull(result);
        Assert.Equal("secret connection", result.ConnectionString);
    }

    [Fact]
    public async Task GetSecretValueAsync_SecretNotFound_ThrowsException()
    {
        var client = new SecretsManagerMockBuilder()
            .WithSecretString(string.Empty)
            .Build();

        Func<Task> testCode = () => new SecretManager(client.Object).GetSecretValueAsync<DBConnection>(SecretsManagerMockBuilder.SecretName);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(testCode);
        Assert.Equal(NullSecretStringExceptionMessage, exception.Message);
    }

    [Fact]
    public async Task GetDbConnectionAsync_ValidSecret_ReturnsConnectionString()
    {
        Environment.SetEnvironmentVariable("SECRET_NAME_OF_CONNECTION", SecretsManagerMockBuilder.SecretName);
        var client = new SecretsManagerMockBuilder()
            .Build();

        var result = await new SecretManager(client.Object).GetDbConnectionAsync();

        Assert.Equal("secret connection", result);
    }

    [Fact]
    public async Task GetDbConnectionAsync_SecretNotFound_ThrowsException()
    {
        Environment.SetEnvironmentVariable("SECRET_NAME_OF_CONNECTION", "");
        var client = new SecretsManagerMockBuilder()
            .Build();

        Func<Task> testCode = () => new SecretManager(client.Object).GetDbConnectionAsync();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(testCode);
        Assert.Equal(EnvironmentNotSetExceptionMessage("SECRET_NAME_OF_CONNECTION"), exception.Message);
    }

    [Fact]
    public async Task GetDbConnectionAsync_ConnectionStringMissing_ThrowsException()
    {
        Environment.SetEnvironmentVariable("SECRET_NAME_OF_CONNECTION", SecretsManagerMockBuilder.SecretName);
        var client = new SecretsManagerMockBuilder()
            .WithSecretString("{\"connectionString\": \"\"}")
            .Build();

        Func<Task> testCode = () => new SecretManager(client.Object).GetDbConnectionAsync();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(testCode);
        Assert.Equal(NullConnectionStringExceptionMessage, exception.Message);
    }
}