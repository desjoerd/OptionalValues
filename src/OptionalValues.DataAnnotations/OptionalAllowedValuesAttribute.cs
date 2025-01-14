using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalAllowedValuesAttribute : AllowedValuesAttribute
{
    /// <inheritdoc />
    public OptionalAllowedValuesAttribute(
        [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "In the base class")]
        params object[] allowedValues)
        : base(allowedValues)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}