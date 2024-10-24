using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptionalValues;

/// <summary>
/// JsonConverter for <see cref="OptionalValue{T}"/>.
///
/// When Reading, if a value is omitted, it will be considered unspecified.
///
/// When Writing, if a value is unspecified, it will emit default.
/// For this behavior to work, the property must be marked with [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)].
/// </summary>
/// <remarks>Requires the use of <see cref="JsonIgnoreAttribute"/> with <see cref="JsonIgnoreAttribute.Condition"/> set to <see cref="JsonIgnoreCondition.WhenWritingDefault"/> on every property which is a <see cref="OptionalValue{T}"/></remarks>
/// <typeparam name="T"></typeparam>
[SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Class is instantiated via reflection by OptionalValueJsonConverterFactory.")]
internal sealed class OptionalValueJsonConverter<T> : JsonConverter<OptionalValue<T>>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    /// <inheritdoc />
    public override OptionalValue<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<T>(ref reader, options)!;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, OptionalValue<T> value, JsonSerializerOptions options)
    {
        if (!value.IsSpecified)
        {
            // We cannot tell the serialization to ignore outputting the property.
            // If we would not write anything, we end up with invalid JSON.
            // IE: { "value": }
            // So we throw an exception to indicate that the value is undefined.
            throw new InvalidOperationException("Value is unspecified. Please set [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] on the property.");
        }

        // Write the value or null if the value is null
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}

