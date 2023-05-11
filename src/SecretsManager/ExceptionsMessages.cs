namespace Helpers.Secrets.Exceptions;

public static class ExceptionsMessages
{
    public static string SecretNotFound() => "Secret name not found.";
    public static string DeserializeSecretError<T>() where T : class =>
        $"Error occurred on deserialize secret JSON to '{typeof(T).Name}' model. Deserialize result is null.";

    public static string SecretWithoutData(string secretName) =>
        $"Secret '{secretName}' does not contain any data.";
}
