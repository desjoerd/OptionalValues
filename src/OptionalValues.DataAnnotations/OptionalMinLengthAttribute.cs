using System.ComponentModel.DataAnnotations;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalMinLengthAttribute : MinLengthAttribute
{
    /// <inheritdoc />
    public OptionalMinLengthAttribute(int length)
        : base(length)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}