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
    /// <remarks>This should preferably be done as the last call, as it applies a modifier to the registered <see cref="JsonTypeInfoResolver"/> instances in the TypeInfoResolverChain.</remarks>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to modify.</param>
    /// <returns>The modified <see cref="JsonSerializerOptions"/> to allow for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static JsonSerializerOptions AddOptionalValueSupport(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // If the options do not have a TypeInfoResolver, add the default one, with the modifier.
        options.TypeInfoResolver ??= JsonSerializer.IsReflectionEnabledByDefault
            ? new DefaultJsonTypeInfoResolver()
            : JsonTypeInfoResolver.Combine();

        // We need to add the modifier to all resolvers in the chain,
        // because it needs to be applied to all types and it's properties.
        for (var i = 0; i < options.TypeInfoResolverChain.Count; i++)
        {
            options.TypeInfoResolverChain[i] = options.TypeInfoResolverChain[i]
                .WithOptionalValueSupport();
        }

        return options;
    }

    /// <summary>
    /// Creates a new clone of <see cref="JsonSerializerOptions"/> with support for <see cref="OptionalValue{T}"/>.
    /// </summary>
    /// <param name="options">The base <see cref="JsonSerializerOptions"/> options to copy.</param>
    /// <returns>A new <see cref="JsonSerializerOptions"/> based on the provided options with support for <see cref="OptionalValue{T}"/>.</returns>
    public static JsonSerializerOptions WithOptionalValueSupport(this JsonSerializerOptions options)
        => new JsonSerializerOptions(options)
            .AddOptionalValueSupport();

    private static IJsonTypeInfoResolver WithOptionalValueSupport(this IJsonTypeInfoResolver resolver)
        => resolver.WithAddedModifier(OptionalValueJsonTypeInfoResolverModifier.ModifyTypeInfo);
}