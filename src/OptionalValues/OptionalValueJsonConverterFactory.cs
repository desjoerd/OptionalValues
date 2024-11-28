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
internal sealed class OptionalValueJsonConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Checks if the given type is a <see cref="OptionalValue{T}"/>.
    /// </summary>
    /// <param name="typeToConvert"></param>
    /// <returns></returns>
    public override bool CanConvert(Type typeToConvert)
        => OptionalValue.IsOptionalValueType(typeToConvert);

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = OptionalValue.GetUnderlyingType(typeToConvert);

        // Create the specific DefinedJsonConverter<T> for the given T
        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(OptionalValueJsonConverter<>).MakeGenericType(valueType)
        )!;

        return converter;
    }
}