using System.ComponentModel.DataAnnotations;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalRegularExpressionAttribute : RegularExpressionAttribute
{
    /// <inheritdoc />
    public OptionalRegularExpressionAttribute(string pattern) : base(pattern)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}