namespace OptionalValues.Extensions;

/// <summary>
/// Extension methods for <see cref="IDictionary{TKey, TValue}"/>.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the value associated with the specified key, returning <see cref="OptionalValue{T}.Unspecified"/> if the key is not found.
    /// </summary>
    /// <returns>A specified <see cref="OptionalValue{T}"/> when the key is found, or an <see cref="OptionalValue{T}.Unspecified"/> when the key is not found.</returns>
    public static OptionalValue<T> GetOptionalValue<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(key);

        return dictionary.TryGetValue(key, out T? value) ? new OptionalValue<T>(value) : OptionalValue<T>.Unspecified;
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary if the value is specified.
    /// </summary>
    /// <exception cref="ArgumentException">When value is specified and the key already exists in the dictionary.</exception>
    /// <exception cref="ArgumentNullException">The key is null.</exception>
    public static void AddOptionalValue<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key, OptionalValue<T> value)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(key);

        if (value.IsSpecified)
        {
            dictionary.Add(key, value.SpecifiedValue);
        }
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary if the value is specified.
    /// </summary>
    /// <exception cref="ArgumentException">When value is specified and the key already exists in the dictionary.</exception>
    /// <exception cref="ArgumentNullException">The key is null.</exception>
    /// <returns><see langword="true"/> if the value was added to the dictionary; otherwise when the <paramref name="key"/> already exists or the value is <see cref="OptionalValue{T}.Unspecified"/>, <see langword="false"/>.</returns>
    public static bool TryAddOptionalValue<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key, OptionalValue<T> value)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(key);

        return value.IsSpecified && dictionary.TryAdd(key, value.SpecifiedValue);
    }

    /// <summary>
    /// Sets the specified key and value in the dictionary if the value is specified.
    /// </summary>
    /// <exception cref="ArgumentNullException">The key is null.</exception>
    public static void SetOptionalValue<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key, OptionalValue<T> value)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(key);

        if (value.IsSpecified)
        {
            dictionary[key] = value.SpecifiedValue;
        }
    }
}