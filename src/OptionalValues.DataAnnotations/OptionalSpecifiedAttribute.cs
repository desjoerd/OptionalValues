using System.ComponentModel.DataAnnotations;

using OptionalValues.Internal;

namespace OptionalValues.DataAnnotations;

/// <summary>
/// Validation attribute to indicate that a property, field or parameter must be a Specified <see cref="OptionalValue{T}" />.
/// </summary>
/// <remarks>Consider NOT using an <see cref="OptionalValue{T}"/> and use the <c>required</c> keyword instead when you want to enforce the property to be <c>Specified</c>.</remarks>
public class OptionalSpecifiedAttribute : RequiredAttribute
{
    [Obsolete("This property is not used in this attribute. It is only present to match the base class API.", true)]
    public new bool AllowEmptyStrings
    {
        get => base.AllowEmptyStrings;
        set => base.AllowEmptyStrings = value;
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        if (value is IOptionalValueInternals optionalValue)
        {
            return optionalValue.IsSpecified;
        }

        return true;
    }
}
