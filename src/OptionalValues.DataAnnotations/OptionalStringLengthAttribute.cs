using System.ComponentModel.DataAnnotations;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalStringLengthAttribute : StringLengthAttribute
{
    /// <inheritdoc />
    public OptionalStringLengthAttribute(int maximumLength) : base(maximumLength)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}