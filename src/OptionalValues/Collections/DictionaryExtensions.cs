namespace OptionalValues.Collections;

public static class DictionaryExtensions
{
    public static OptionalValue<T> GetOptionalValue<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key)
        => dictionary.TryGetValue(key, out T? value) ? new OptionalValue<T>(value) : OptionalValue<T>.Unspecified;

    public static void AddOptionalValue<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key, OptionalValue<T> value)
    {
        if (value.IsSpecified)
        {
            dictionary.Add(key, value.SpecifiedValue);
        }
    }
}