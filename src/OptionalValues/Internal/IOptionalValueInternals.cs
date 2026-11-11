namespace OptionalValues.Internal;

internal interface IOptionalValueInternals : IOptionalValue
{
    object? GetValue();

    object? GetSpecifiedValue();
}