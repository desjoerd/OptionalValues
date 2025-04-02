using System.ComponentModel.DataAnnotations;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalBase64StringAttribute : Base64StringAttribute
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}