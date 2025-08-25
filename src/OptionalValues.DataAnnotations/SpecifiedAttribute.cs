using System.ComponentModel.DataAnnotations;

using OptionalValues.Internal;

namespace OptionalValues.DataAnnotations;

/// <summary>
/// Validation attribute to indicate that a property, field or parameter must be a Specified <see cref="OptionalValue{T}" />. It still allows <c>null</c> values and empty strings.
/// </summary>
/// <remarks>Consider NOT using an <see cref="OptionalValue{T}"/> and use the <c>required</c> keyword instead when you want to enforce the property to be <c>Specified</c>.</remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SpecifiedAttribute()
    : ValidationAttribute("The {0} field must be specified.")
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
        => value is not IOptionalValueInternals { IsSpecified: false };
}
