using System.Text.Json.Serialization.Metadata;

using Microsoft.AspNetCore.OpenApi;

namespace OptionalValues.OpenApi;

/// <summary>
/// Extension methods for <see cref="OpenApiOptions"/> to add support for <see cref="OptionalValue{T}"/> types.
/// </summary>
public static class OpenApiOptionsExtensions
{
    /// <summary>
    /// Adds support for <see cref="OptionalValue{T}"/> types in OpenAPI generation.
    /// This method configures the <see cref="OpenApiOptions"/> to correctly handle
    /// <see cref="OptionalValue{T}"/> types by applying a custom schema reference ID
    /// creation logic and adding a schema transformer.
    /// </summary>
    /// <param name="options">The OpenApiOptions instance to configure.</param>
    public static void AddOptionalValueSupport(this OpenApiOptions options)
    {
        options.ApplyOptionalValueCreateSchemaReferenceId();
        options.AddSchemaTransformer<OptionalValuesSchemaTransformer>();
    }

    private static void ApplyOptionalValueCreateSchemaReferenceId(this OpenApiOptions options)
    {
        Func<JsonTypeInfo, string?> originalCreateSchemaReferenceId = options.CreateSchemaReferenceId;

        options.CreateSchemaReferenceId = jsonTypeInfo =>
        {
            if (OptionalValue.IsOptionalValueType(jsonTypeInfo.Type))
            {
                Type underlyingType = OptionalValue.GetUnderlyingType(jsonTypeInfo.Type);
                var underlyingJsonTypeInfo = JsonTypeInfo.CreateJsonTypeInfo(underlyingType, jsonTypeInfo.Options);

                return originalCreateSchemaReferenceId(underlyingJsonTypeInfo);
            }

            return originalCreateSchemaReferenceId(jsonTypeInfo);
        };
    }
}