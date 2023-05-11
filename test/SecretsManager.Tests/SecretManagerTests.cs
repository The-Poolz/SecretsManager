using Xunit;
using Amazon;
using Amazon.SecretsManager;
using SecretsManager.Models;
using SecretsManager.Tests.Builders;

namespace SecretsManager.Tests;

public class SecretManagerTests
{
    [Fact]
    public void CreateClient_ValidRegion_ReturnsSecretsManagerClient()
    {
        var expectedRegionEndpoint = RegionEndpoint.AFSouth1;
        Environment.SetEnvironmentVariable("AWS_REGION", expectedRegionEndpoint.SystemName);

        var result = SecretManager.CreateClient();

        Assert.NotNull(result);
        Assert.Equal(expectedRegionEndpoint, result.Config.RegionEndpoint);
    }

    [Fact]
    public async Task GetSecretAsync_ValidSecret_ReturnsSecretString()
    {
        var secretName = "validSecret";
        var secretValue = "secretData";
        var client = new SecretsManagerMockBuilder()
            .WithSecretName(secretName)
            .WithSecretString(secretValue)
            .Build();

        var result = await SecretManager.GetSecretAsync(secretName, client.Object);

        Assert.Equal(secretValue, result);
    }

    [Fact]
    public async Task GetSecretAsync_SecretNotFound_ReturnsNull()
    {
        var secretName = "nonExistentSecret";
        var client = new SecretsManagerMockBuilder()
            .Build();

        Func<Task> testCode = () => SecretManager.GetSecretAsync(secretName, client.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
        Assert.Equal("Value cannot be null. (Parameter 'SecretString')", exception.Message);
    }

    [Fact]
    public async Task GetSecretAsync_ExceptionThrown_ThrowsException()
    {
        var client = new SecretsManagerMockBuilder()
            .WithException()
            .Build();

        Func<Task> testCode = () => SecretManager.GetSecretAsync("secret", client.Object);

        var exception = await Assert.ThrowsAsync<AmazonSecretsManagerException>(testCode);
        Assert.Equal("This should not be called", exception.Message);
    }

    [Fact]
    public async Task GetSecretValueAsync_ValidSecret_ReturnsDeserializedSecret()
    {
        var secretName = "ConnectionToDB";
        var client = new SecretsManagerMockBuilder()
            .WithSecretName(secretName)
            .Build();

        var result = await SecretManager.GetSecretValueAsync<DBConnection>(secretName, client.Object);

        Assert.NotNull(result);
        Assert.Equal("secret connection", result.ConnectionString);
    }

    [Fact]
    public async Task GetSecretValueAsync_SecretNotFound_ThrowsException()
    {
        var secretName = "ConnectionToDB";
        var client = new SecretsManagerMockBuilder()
            .WithSecretName(secretName)
            .WithSecretString(string.Empty)
            .Build();

        Func<Task> testCode = () => SecretManager.GetSecretValueAsync<DBConnection>(secretName, client.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
        Assert.Equal("Value cannot be null. (Parameter 'SecretString')", exception.Message);
    }

    [Fact]
    public async Task GetDbConnectionAsync_ValidSecret_ReturnsConnectionString()
    {
        Environment.SetEnvironmentVariable("SECRET_NAME_OF_CONNECTION", "ConnectionToDB");
        var client = new SecretsManagerMockBuilder()
            .Build();

        var result = await SecretManager.GetDbConnectionAsync(client.Object);

        Assert.Equal("secret connection", result);
    }

    [Fact]
    public async Task GetDbConnectionAsync_SecretNotFound_ThrowsException()
    {
        Environment.SetEnvironmentVariable("SECRET_NAME_OF_CONNECTION", "");
        var client = new SecretsManagerMockBuilder()
            .Build();

        Func<Task> testCode = () => SecretManager.GetDbConnectionAsync(client.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
        Assert.Equal("Value cannot be null. (Parameter 'SECRET_NAME_OF_CONNECTION')", exception.Message);
    }

    [Fact]
    public async Task GetDbConnectionAsync_ConnectionStringMissing_ThrowsException()
    {
        Environment.SetEnvironmentVariable("SECRET_NAME_OF_CONNECTION", "ConnectionToDB");
        var client = new SecretsManagerMockBuilder()
            .WithSecretString("{\"connectionString\": \"\"}")
            .Build();

        Func<Task> testCode = () => SecretManager.GetDbConnectionAsync(client.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(testCode);
        Assert.Equal("Value cannot be null. (Parameter 'ConnectionString')", exception.Message);
    }
}