using System.ComponentModel.DataAnnotations;

using OptionalValues.Internal;

namespace OptionalValues.DataAnnotations.Internal;

internal static class OverrideHelper
{
    internal static bool OptionalIsValid(object? value, Func<object?, bool> baseIsValid)
    {
        if (value is IOptionalValueInternals optionalValue)
        {
            if (!optionalValue.IsSpecified)
            {
                return true;
            }

            value = optionalValue.GetSpecifiedValue();
        }

        return baseIsValid(value);
    }

    internal static ValidationResult? OptionalIsValid(
        object? value,
        ValidationContext validationContext,
        Func<object?, ValidationContext, ValidationResult?> baseIsValid)
    {
        if (value is IOptionalValueInternals optionalValue)
        {
            if (!optionalValue.IsSpecified)
            {
                return ValidationResult.Success;
            }

            value = optionalValue.GetSpecifiedValue();
        }

        return baseIsValid(value, validationContext);
    }
}
