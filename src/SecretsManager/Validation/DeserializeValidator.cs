using Newtonsoft.Json;
using FluentValidation;

namespace SecretsManager.Validation;

public class DeserializeValidator<T> : AbstractValidator<T>
{
    public T DeserializedObject { get; private set; } = default!;

    public DeserializeValidator(string secretResponse)
    {
        RuleFor(x => secretResponse)
            .Must(PossibleDeserialization)
            .WithMessage($"Could not deserialize the secret response to type {typeof(T)}.");

        RuleFor(x => DeserializedObject)
            .NotNull();
    }

    private bool PossibleDeserialization(string secretResponse)
    {
        DeserializedObject = JsonConvert.DeserializeObject<T>(secretResponse)!;
        return DeserializedObject != null;
    }
}
