using System.Diagnostics;

using Namotion.Reflection;

using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;

namespace OptionalValues.NSwag;

/// <summary>
/// Type mapper for <see cref="OptionalValue{T}"/>.
/// </summary>
public class OptionalValueTypeMapper: ITypeMapper
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

        var contextualType = context.Type.ToContextualType(context.ParentAttributes);
        var underlyingContextualType = OptionalValue.GetUnderlyingType(context.Type).ToContextualType(context.ParentAttributes);

        context.JsonSchemaGenerator.Generate(schema, underlyingContextualType, context.JsonSchemaResolver);

        var underlyingIsNullable = contextualType.GenericArguments[0].IsNullableType
                                   || contextualType.GenericArguments[0].Nullability == Nullability.Nullable;
        var jsonTypeDescription = JsonTypeDescription.Create(underlyingContextualType, schema.Type, underlyingIsNullable, schema.Format);
        context.JsonSchemaGenerator.ApplyDataAnnotations(schema, jsonTypeDescription);

        if (underlyingIsNullable)
        {
            schema.IsNullableRaw = true;
        }
    }
}