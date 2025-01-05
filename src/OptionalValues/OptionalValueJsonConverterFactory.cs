using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptionalValues;

/// <summary>
/// This factory creates a <see cref="OptionalValueJsonConverter{T}"/> for each <see cref="OptionalValue{T}"/> type.
/// </summary>
[SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Class is instantiated via reflection by JsonSerializer.")]
public sealed class OptionalValueJsonConverterFactory : JsonConverterFactory
{
    private readonly JsonConverter? _customInnerConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalValueJsonConverterFactory"/> class.
    /// </summary>
    public OptionalValueJsonConverterFactory()
    {
    }

    internal OptionalValueJsonConverterFactory(JsonConverter? customInnerConverter)
    {
        _customInnerConverter = customInnerConverter;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => OptionalValue.IsOptionalValueType(typeToConvert);

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Method is called by JsonSerializer and is never null.")]
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = OptionalValue.GetUnderlyingType(typeToConvert);

        JsonConverter? inner = _customInnerConverter ?? options.GetConverter(valueType);
        if (inner is JsonConverterFactory factory)
        {
            inner = factory.CreateConverter(valueType, options);
        }

        switch (inner)
        {
            case null:
                throw new InvalidOperationException($"No converter found for {valueType}.");
            case JsonConverterFactory:
                throw new InvalidOperationException($"Converter for {valueType} is a factory which returned a factory.");
        }

        // Create the specific OptionalValueJsonConverter<T> for the given T
        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(OptionalValueJsonConverter<>).MakeGenericType(valueType), inner
        )!;

        return converter;
    }
}