namespace OptionalValues;

/// <summary>
/// Represents an unspecified value. This can be used in pattern matching to distinguish between a value that is not specified and a value that is specified.
/// </summary>
public readonly struct Unspecified
{
    /// <summary>
    /// An unspecified value.
    /// </summary>
    public static readonly Unspecified Value = new();

    /// <inheritdoc />
    override public string ToString() => "Unspecified";

    /// <inheritdoc />
    override public int GetHashCode() => Int32.MinValue;
}
