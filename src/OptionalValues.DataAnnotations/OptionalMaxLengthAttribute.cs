using System.ComponentModel.DataAnnotations;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalMaxLengthAttribute : MaxLengthAttribute
{
    /// <inheritdoc />
    public OptionalMaxLengthAttribute()
    {
    }

    /// <inheritdoc />
    public OptionalMaxLengthAttribute(int length)
        : base(length)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}