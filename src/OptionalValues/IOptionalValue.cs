namespace OptionalValues;

/// <summary>
/// Represents an optional value that may or may not be specified.
/// </summary>
public interface IOptionalValue
{
    /// <summary>
    /// Whether the value is specified.
    /// </summary>
    bool IsSpecified { get; }
}
