using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;

namespace OptionalValues;

/// <summary>
/// When placed on a property or field of type <see cref="OptionalValue{T}"/>, specifies the converter type to use for <c>T</c>.
/// </summary>
/// <remarks>
/// The specified converter type must derive from JsonConverter.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
[SuppressMessage("Performance", "CA1813:Avoid unsealed attributes", Justification = "This attribute should be inheritable to allow custom initialization logic.")]
public class OptionalValueJsonConverterAttribute : JsonConverterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="OptionalValueJsonConverterAttribute"/> with the specified inner converter type.
    /// </summary>
    /// <param name="innerConverterType"></param>
    public OptionalValueJsonConverterAttribute(Type innerConverterType)
    {
        InnerConverterType = innerConverterType;
    }

    /// <summary>
    /// Protected constructor for derived classes, allowing to create custom logic for creating the inner converter.
    /// </summary>
    protected OptionalValueJsonConverterAttribute()
    {
    }

    /// <summary>
    /// Gets the type of the inner converter. This can be null if <see cref="CreateInnerConverter"/> is overridden to provide custom logic for creating the inner converter.
    /// </summary>
    public Type? InnerConverterType { get; }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert)
    {
        JsonConverter innerConverter = CreateInnerConverter();

        return (JsonConverter?)Activator.CreateInstance(
            type: typeof(OptionalValueJsonConverterFactory),
            bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
            args: [innerConverter],
            binder: null,
            culture: null
        );
    }

    /// <summary>
    /// Creates the inner converter. This method can be overridden in derived classes to provide custom logic for creating the inner converter.
    /// </summary>
    /// <returns>A <see cref="JsonConverter{T}"/> or <see cref="JsonConverterFactory"/></returns>
    protected virtual JsonConverter CreateInnerConverter()
    {
        if (InnerConverterType is null)
        {
            throw new InvalidOperationException("When inheriting from OptionalValueJsonConverterAttribute, the CreateInnerConverter method must be overridden.");
        }
        return (JsonConverter)Activator.CreateInstance(InnerConverterType)!;
    }
}