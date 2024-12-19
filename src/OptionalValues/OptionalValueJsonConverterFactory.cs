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
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => OptionalValue.IsOptionalValueType(typeToConvert);

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Method is called by JsonSerializer and is never null.")]
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = OptionalValue.GetUnderlyingType(typeToConvert);

        JsonConverter inner = options.GetConverter(valueType);

        // Create the specific DefinedJsonConverter<T> for the given T
        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(OptionalValueJsonConverter<>).MakeGenericType(valueType), inner
        )!;

        return converter;
    }
}