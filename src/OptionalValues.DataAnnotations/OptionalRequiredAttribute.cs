using System.ComponentModel.DataAnnotations;

using OptionalValues.Internal;

namespace OptionalValues.DataAnnotations;

/// <summary>
/// Validation attribute to indicate that a property, field or parameter is required.
/// </summary>
/// <remarks>Consider NOT using a <see cref="OptionalValue{T}"/> and use the <c>required</c> keyword instead when you want to enforce the property to be <c>Specified</c>.</remarks>
public class OptionalRequiredAttribute : RequiredAttribute
{
    /// <summary>
    /// Whether the property is allowed to be Unspecified. That is <c>OptionalValue.IsSpecified == false</c>.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool AllowUnspecified { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        if (value is IOptionalValueInternals optionalValue)
        {
            if (!optionalValue.IsSpecified)
            {
                return AllowUnspecified;
            }

            value = optionalValue.GetSpecifiedValue();
        }

        return base.IsValid(value);
    }
}