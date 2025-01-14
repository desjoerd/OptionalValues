namespace OptionalValues.Internal;

internal interface IOptionalValueInternals
{
    bool IsSpecified { get; }

    object? GetValue();

    object? GetSpecifiedValue();
}