# SecretsManager

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/summary/new_code?id=The-Poolz_SecretsManager)

[![SonarScanner for .NET 6](https://github.com/The-Poolz/SecretsManager/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/The-Poolz/SecretsManager/actions/workflows/dotnet.yml)
[![CodeFactor](https://www.codefactor.io/repository/github/the-poolz/secretsmanager/badge)](https://www.codefactor.io/repository/github/the-poolz/secretsmanager)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=The-Poolz_SecretsManager&metric=alert_status&token=372f9dddfc2bc5547a55e9a85cf072de406df1de)](https://sonarcloud.io/summary/new_code?id=The-Poolz_SecretsManager)

## How to use

```csharp
// Create an instance of the AWS Secrets Manager client
var client = SecretManager.CreateClient();

// Create an instance of the SecretManager class
var secretManager = new SecretManager(client);

// Retrieve the value of a secret
var secretValue = secretManager.GetSecretValue("mySecretId", "mySecretKey");
```

## `SecretManager` class

This class provides a way to manage secrets using AWS Secrets Manager.

### Properties

- `client` (IAmazonSecretsManager): The AWS Secrets Manager client used for managing secrets.

### Constructors

- `SecretManager()`: Initializes a new instance of the `SecretManager` class using the default AWS Secrets Manager client.
- `SecretManager(IAmazonSecretsManager client)`: Initializes a new instance of the `SecretManager` class using the specified AWS Secrets Manager client.

### Methods

#### GetSecretValue(string secretId, string secretKey)

Retrieves the value of a secret identified by `secretId` and returns the value associated with `secretKey`.

- `secretId` (string): The ID or name of the secret.
- `secretKey` (string): The key used to retrieve the secret value.

Returns:
- The secret value associated with `secretKey`.

Exceptions:

- `KeyNotFoundException`: Thrown when the specified secret key does not exist.

#### CreateClient()

Creates an instance of the AWS Secrets Manager client using the region specified in the `AWS_REGION` environment variable or the default region (`RegionEndpoint.USEast1`).

Returns:

- An instance of `IAmazonSecretsManager` representing the AWS Secrets Manager client.

## `RegionProvider` class

This class provides methods for retrieving the AWS region endpoint.

### Properties

- `DefaultRegion` (RegionEndpoint): The default AWS region endpoint.

### Methods

#### GetRegionEndpoint()

Retrieves the AWS region endpoint based on the `AWS_REGION` environment variable or returns the default region if the environment variable is not set.

Returns:

- The AWS region endpoint.
