using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace OptionalValues;

/// <summary>
/// Extension methods for adding support for <see cref="OptionalValue{T}"/> to <see cref="JsonSerializerOptions"/>.
/// </summary>
public static class OptionalValueJsonExtensions
{
    /// <summary>
    /// Modifies the provided <see cref="JsonSerializerOptions"/> to add support for <see cref="OptionalValue{T}"/>.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to modify.</param>
    /// <returns>The modified <see cref="JsonSerializerOptions"/> to allow for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static JsonSerializerOptions AddOptionalValueSupport(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.TypeInfoResolver = (options.TypeInfoResolver ?? new DefaultJsonTypeInfoResolver())
            .WithAddedModifier(OptionalValueJsonTypeInfoResolverModifier.ModifyTypeInfo);

        return options;
    }

    /// <summary>
    /// Creates a new <see cref="JsonSerializerOptions"/> with support for <see cref="OptionalValue{T}"/>.
    /// </summary>
    /// <param name="options">The base <see cref="JsonSerializerOptions"/> options to copy.</param>
    /// <returns>A new <see cref="JsonSerializerOptions"/> based on the provided options with support for <see cref="OptionalValue{T}"/>.</returns>
    public static JsonSerializerOptions WithOptionalValueSupport(this JsonSerializerOptions options) =>
        new JsonSerializerOptions(options)
            .AddOptionalValueSupport();
}