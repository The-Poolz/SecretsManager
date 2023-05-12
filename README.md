# SecretsManager

## `SecretManager` class

This class provides a way to manage secrets, specifically database connection strings, using AWS Secret Manager.

### Properties

- `client` (IAmazonSecretsManager): The AWS Secrets Manager client used for managing secrets.

### Constructors

- `SecretManager(IAmazonSecretsManager client)`: Creates a new `SecretManager` instance using the specified AWS Secrets Manager client.

### Exceptions
- `InvalidOperationException`: This exception is thrown when the required environment variables are not set, or when the secret value or connection string is null or empty.
It is also thrown when a secret value cannot be deserialized to the specified type.

### Methods

#### GetDbConnectionAsync()

Asynchronously retrieves a database connection string. The name of the secret is retrieved from the `SECRET_NAME_OF_CONNECTION` environment variable.

Example
```csharp
// Create client using environment variable
var client = SecretManager.CreateClient();

var connection = await new SecretManager(client).GetDbConnectionAsync();
```

#### GetSecretValueAsync<T>(string secretName)

Asynchronously retrieves a secret value of the specified type. The secret value is deserialized from JSON format.

Example
```csharp
// Create client using environment variable
var client = SecretManager.CreateClient();

// Secret name need be set in AWS Secret Manager
// See docs: https://docs.aws.amazon.com/secretsmanager/latest/userguide/intro.html
var secretName = "YourSecretName"

// YourModel - it's model representing data from 'YourSecretName'
var connection = await new SecretManager(client).GetSecretValueAsync<YourModel>(secretName);
```

#### GetSecretAsync(string secretName)

Asynchronously retrieves a secret value as a string.

Example
```csharp
// Create client using environment variable
var client = SecretManager.CreateClient();

// Secret name need be set in AWS Secret Manager
// See docs: https://docs.aws.amazon.com/secretsmanager/latest/userguide/intro.html
var secretName = "YourSecretName"

var connection = await new SecretManager(client).GetSecretAsync(secretName);
```

#### CreateClient(RegionEndpoint? region = null)

Creates an IAmazonSecretsManager client. The AWS region is retrieved from the `AWS_REGION` environment variable, or can be provided as an argument.

Example
```csharp
// Create client using environment variable
// AWS platform contain system environment 'AWS_REGION' who contain region name where code be launch
var client = SecretManager.CreateClient();

// You can pass region if needed
var client = SecretManager.CreateClient(RegionEndpoint.AFSouth1);
```
