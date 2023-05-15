using Newtonsoft.Json;

namespace SecretsManager.Serialization;

public static class JsonDeserializer
{
    public static T Deserialize<T>(string secretResponse)
    {
        T? deserializedObject = JsonConvert.DeserializeObject<T>(secretResponse);

        if (deserializedObject == null)
        {
            throw new InvalidOperationException($"Could not deserialize the secret response to type {typeof(T)}.");
        }

        return deserializedObject;
    }
}
