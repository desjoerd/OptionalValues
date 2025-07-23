using System.Diagnostics;

using Namotion.Reflection;

using NJsonSchema;
using NJsonSchema.Annotations;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;

namespace OptionalValues.NSwag;

/// <summary>
/// Type mapper for <see cref="OptionalValue{T}"/>.
/// </summary>
public class OptionalValueTypeMapper : ITypeMapper
{
    /// <inheritdoc />
    public Type MappedType => typeof(OptionalValue<>);

    /// <inheritdoc />
    public bool UseReference => false;

    /// <inheritdoc />
    public void GenerateSchema(JsonSchema schema, TypeMapperContext context)
    {
        Debug.Assert(schema != null, nameof(schema) + " != null");
        Debug.Assert(context != null, nameof(context) + " != null");

        JsonSchemaGeneratorSettings settings = context.JsonSchemaGenerator.Settings;

        var contextualType = context.Type.ToContextualType(context.ParentAttributes);
        var underlyingIsNullable = contextualType.GenericArguments[0].IsNullableType
                                   || contextualType.GenericArguments[0].Nullability == Nullability.Nullable;

        Type underLyingType = OptionalValue.GetUnderlyingType(context.Type);

        var underlyingContextualType = underLyingType.ToContextualType(context.ParentAttributes.Concat([
            new JsonSchemaTypeAttribute(underLyingType)
            {
                IsNullable = underlyingIsNullable
            },
        ]));

        context.JsonSchemaGenerator.Generate(schema, underlyingContextualType, context.JsonSchemaResolver);

        var applyNullableRaw = settings.SchemaType != SchemaType.JsonSchema &&
                               (settings.SchemaType == SchemaType.OpenApi3 || settings.GenerateCustomNullableProperties);
        if (applyNullableRaw && underlyingIsNullable)
        {
            schema.IsNullableRaw = underlyingIsNullable;
        }

        var jsonTypeDescription = JsonTypeDescription.Create(underlyingContextualType, schema.Type, underlyingIsNullable, schema.Format);
        context.JsonSchemaGenerator.ApplyDataAnnotations(schema, jsonTypeDescription);
    }
}