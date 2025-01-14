using System.ComponentModel.DataAnnotations;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalLengthAttribute : LengthAttribute
{
    /// <inheritdoc />
    public OptionalLengthAttribute(int minimumLength, int maximumLength) : base(minimumLength, maximumLength)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}