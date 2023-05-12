using Xunit;
using FluentValidation;
using SecretsManager.Tests.Models;
using SecretsManager.Tests.Builders;

namespace SecretsManager.Tests;

public class SecretManagerTests
{
    private static string DeserializeExceptionMessage =>
        $"Validation failed: {Environment.NewLine} -- secretResponse: " +
        $"Could not deserialize the secret response to type {typeof(DBConnection)}. " +
        $"Severity: Error{Environment.NewLine} -- DeserializedObject: " +
        $"'Deserialized Object' must not be empty. Severity: Error";

    [Fact]
    public void GetSecretValue_ValidSecret_ReturnsDeserializedSecret()
    {
        var client = new SecretsManagerMockBuilder()
            .Build();

        var result = new SecretManager(client.Object).GetSecretValue(SecretsManagerMockBuilder.SecretName, new DBConnection());

        Assert.NotNull(result);
        Assert.Equal("secret connection", result.ConnectionString);
    }

    [Fact]
    public void GetSecretValue_SecretNotFound_ThrowsException()
    {
        var client = new SecretsManagerMockBuilder()
            .WithSecretString(string.Empty)
            .Build();

        Action testCode = () => new SecretManager(client.Object).GetSecretValue(SecretsManagerMockBuilder.SecretName, new DBConnection());

        var exception = Assert.Throws<ValidationException>(testCode);
        Assert.Equal(DeserializeExceptionMessage, exception.Message);
    }
}