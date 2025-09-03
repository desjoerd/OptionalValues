using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using OptionalValues.Internal;

namespace OptionalValues;

/// <summary>
/// A value that may or may not be specified. This is different from a <see cref="Nullable{T}"/> value type, as it allows for the distinction between a value that is null and a value that is unspecified.
/// </summary>
/// <typeparam name="T">The type of the value, can be <see cref="Nullable{T}"/></typeparam>
[JsonConverter(typeof(OptionalValueJsonConverterFactory))]
public readonly struct OptionalValue<T> : IEquatable<OptionalValue<T>>, IOptionalValueInternals
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
    public T? GetSpecifiedValueOrDefault() => GetSpecifiedValueOrDefault(default);

    /// <summary>
    /// Gets the specified value or the provided default value.
    /// </summary>
    /// <param name="defaultValue">The value to return when <see cref="IsSpecified" /> is <c>false</c></param>
    /// <returns>
    /// The specified value or the <paramref name="defaultValue"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public T? GetSpecifiedValueOrDefault(T? defaultValue) => IsSpecified ? Value! : defaultValue;

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
    /// Determines whether the other OptionalValue is equal to the current OptionalValue.
    /// </summary>
    /// <param name="other">The other OptionalValue to compare.</param>
    /// <returns><c>true</c> if the other OptionalValue is equal to the current OptionalValue; otherwise, <c>false</c>.</returns>
    public bool Equals(OptionalValue<T> other)
    {
        if (!IsSpecified)
        {
            return !other.IsSpecified;
        }
        return other.IsSpecified && EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj switch
        {
            OptionalValue<T> other => Equals(other),
            T value => Equals(new OptionalValue<T>(value)),
            null => IsSpecified && Value is null,
            _ => false,
        };

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (IsSpecified)
        {
            if (Value is null)
            {
                return 0;
            }
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        // unspecified, a different value than null
        return Int32.MinValue;
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

    /// <summary>
    /// Overloads the equality operator for OptionalValue.
    /// </summary>
    /// <param name="left">Value to compare to right</param>
    /// <param name="right">Value to compare to left</param>
    /// <returns><c>true</c> if the values are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(OptionalValue<T> left, OptionalValue<T> right) => left.Equals(right);

    /// <summary>
    /// Overloads the inequality operator for OptionalValue.
    /// </summary>
    /// <param name="left">Value to compare to right</param>
    /// <param name="right">Value to compare to left</param>
    /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(OptionalValue<T> left, OptionalValue<T> right) => !(left == right);

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

    /// <summary>
    /// Returns the value as object.
    /// </summary>
    /// <remarks>Try not to use this method, as it will box the value and requires the OptionalValue to be boxed.</remarks>
    object? IOptionalValueInternals.GetValue() => Value;

    /// <summary>
    /// Returns the value as object.
    /// </summary>
    /// <remarks>Try not to use this method, as it will box the value and requires the OptionalValue to be boxed.</remarks>
    object? IOptionalValueInternals.GetSpecifiedValue() => SpecifiedValue;
}

/// <summary>
/// Provides a set of static methods for working with <see cref="OptionalValue{T}"/> types.
/// </summary>
public static class OptionalValue
{
    /// <summary>
    /// Returns the underlying type argument of the specified OptionalValue type.
    /// </summary>
    /// <param name="optionalValueType">The OptionalValue type to get the underlying type of.</param>
    /// <returns>The underlying type of the OptionalValue type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionalValueType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="optionalValueType"/> is not an OptionalValue type.</exception>
    public static Type GetUnderlyingType(Type optionalValueType)
    {
        ArgumentNullException.ThrowIfNull(optionalValueType);

        if (!IsOptionalValueType(optionalValueType))
        {
            throw new ArgumentException("The specified type is not an OptionalValue type.", nameof(optionalValueType));
        }

        return optionalValueType.GetGenericArguments()[0];
    }

    /// <summary>
    /// Determines whether the specified type is an OptionalValue type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the specified type is an OptionalValue type; otherwise, <c>false</c>.</returns>
    public static bool IsOptionalValueType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(OptionalValue<>);
    }
}