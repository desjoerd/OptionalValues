using System.ComponentModel.DataAnnotations;

using OptionalValues.Internal;

namespace OptionalValues.DataAnnotations;

/// <summary>
/// Validation attribute to indicate that a property, field or parameter is required.
/// </summary>
/// <remarks>Consider NOT using an <see cref="OptionalValue{T}"/> and use the <c>required</c> keyword instead when you want to enforce the property to be <c>Specified</c>.</remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class RequiredValueAttribute : RequiredAttribute
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        if (value is IOptionalValueInternals optionalValue)
        {
            if (!optionalValue.IsSpecified)
            {
                return false;
            }

            value = optionalValue.GetSpecifiedValue();
        }

        return base.IsValid(value);
    }
}
