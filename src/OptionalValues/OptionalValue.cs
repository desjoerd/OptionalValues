using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace OptionalValues;

/// <summary>
/// A value that may or may not be specified. This is different from a <see cref="Nullable{T}"/> value type, as it allows for the distinction between a value that is null and a value that is unspecified.
/// </summary>
/// <typeparam name="T">The type of the value, can be <see cref="Nullable{T}"/></typeparam>
[JsonConverter(typeof(OptionalValueJsonConverterFactory))]
public readonly record struct OptionalValue<T>
{
    /// <summary>
    /// Creates an Unspecified OptionalValue.
    /// </summary>
    public OptionalValue()
    {
    }

    /// <summary>
    /// Creates a Specified OptionalValue. That is, a value has been provided.
    /// </summary>
    /// <param name="value">
    /// The specified value. If <typeparamref name="T"/> allows nulls, then this can be null.
    /// </param>
    public OptionalValue(T value)
    {
        IsSpecified = true;
        Value = value;
    }

    /// <summary>
    /// Whether the value is specified.
    /// </summary>
    public bool IsSpecified { get; }

    /// <summary>
    /// The value of the OptionalValue. If <see cref="IsSpecified"/> is false, this will be null.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// The specified value of the OptionalValue. If <see cref="IsSpecified"/> is false, this will throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public T SpecifiedValue => IsSpecified
        ? Value!
        : throw new InvalidOperationException("Value is unspecified.");

    /// <summary>
    /// Gets the specified value or the default value of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// The specified value or the default value of <typeparamref name="T"/>.
    /// </returns>
    public T GetSpecifiedValueOrDefault() => GetSpecifiedValueOrDefault(default!);

    /// <summary>
    /// Gets the specified value or the provided default value.
    /// </summary>
    /// <param name="defaultValue">The value to return when <see cref="IsSpecified" /> is <c>false</c></param>
    /// <returns>
    /// The specified value or the <paramref name="defaultValue"/>.
    /// </returns>
    public T GetSpecifiedValueOrDefault(T defaultValue) => IsSpecified ? Value! : defaultValue;

    /// <summary>
    /// Creates an Unspecified OptionalValue. This will have <see cref="IsSpecified"/> set to <c>false</c>.
    /// </summary>
    /// <remarks>This is the same as <c>default()</c></remarks>
    [SuppressMessage(
        "Design",
        "CA1000:Do not declare static members on generic types",
        Justification = "Having a static class for Unspecified is not adding value at the moment")]
    public static OptionalValue<T> Unspecified => new();

    /// <summary>
    /// Implicitly converts a value to an OptionalValue.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage(
        "Usage",
        "CA2225:Operator overloads have named alternates",
        Justification = "The alternative is the constructor")]
    public static implicit operator OptionalValue<T>(T value) => new(value);

    /// <summary>
    /// Implicitly converts an OptionalValue to a value. If the value is unspecified, this will return null.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage(
        "Usage",
        "CA2225:Operator overloads have named alternates",
        Justification = "The alternative is the constructor")]
    public static implicit operator T?(OptionalValue<T> value) => value.Value;


    /// <inheritdoc />
    public bool Equals(OptionalValue<T> other)
    {
        if (!IsSpecified)
        {
            return !other.IsSpecified;
        }

        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (!IsSpecified || Value is null)
        {
            return 0;
        }

        return EqualityComparer<T>.Default.GetHashCode(Value);
    }

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!IsSpecified)
        {
            return "Unspecified";
        }

        return Value is null ? "Null" : Value.ToString();
    }
}