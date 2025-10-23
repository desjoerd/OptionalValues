using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OptionalValues.OpenApi;

internal class OptionalValuesSchemaTransformer : IOpenApiSchemaTransformer
{
    public async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        TransformObjectSchema(schema, context);
        await TransformPropertySchema(schema, context, cancellationToken);
    }

    private async Task TransformPropertySchema(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonPropertyInfo == null)
        {
            return;
        }

        if (!OptionalValue.IsOptionalValueType(context.JsonPropertyInfo.PropertyType))
        {
            return;
        }

        Type underlyingType = OptionalValue.GetUnderlyingType(context.JsonPropertyInfo.PropertyType);
        OpenApiSchema underlyingSchema = await context.GetOrCreateSchemaAsync(underlyingType, cancellationToken: cancellationToken);

        schema.Type = underlyingSchema.Type;

        schema.Format = underlyingSchema.Format;
        schema.Properties = underlyingSchema.Properties;
        schema.Items = underlyingSchema.Items;
        schema.AnyOf = underlyingSchema.AnyOf;
        schema.AllOf = underlyingSchema.AllOf;
        schema.OneOf = underlyingSchema.OneOf;
        schema.Not = underlyingSchema.Not;
        schema.AdditionalProperties = underlyingSchema.AdditionalProperties;
        schema.Enum = underlyingSchema.Enum;
        schema.AdditionalPropertiesAllowed = underlyingSchema.AdditionalPropertiesAllowed;
        schema.Required = underlyingSchema.Required;

        // Merge annotations
        schema.Description ??= underlyingSchema.Description;
        schema.Default ??= underlyingSchema.Default;
        schema.Example ??= underlyingSchema.Example;

        // Merge the metadata
        if (underlyingSchema.Metadata is not null)
        {
            schema.Metadata ??= new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> keyValuePair in underlyingSchema.Metadata)
            {
                schema.Metadata.TryAdd(keyValuePair.Key, keyValuePair.Value);
            }
        }

        // Patch nullability
        var isUnderlyingIsReferencedSchema = IsSchemaReference(underlyingSchema);

        var customAttributes = context.JsonPropertyInfo.AttributeProvider?.GetCustomAttributes(false) ?? [];
        var isNullable = !customAttributes.OfType<RequiredAttribute>().Any()
                         && GetOptionalValueIsNullable(context.JsonPropertyInfo.AttributeProvider as MemberInfo);
        if (isNullable)
        {
            if (!isUnderlyingIsReferencedSchema)
            {
                schema.Type |= JsonSchemaType.Null;
            }
            else
            {
                schema.Metadata ??= new Dictionary<string, object>();
                schema.Metadata["x-is-nullable-property"] = true;
            }
        }
    }

    private void TransformObjectSchema(OpenApiSchema schema, OpenApiSchemaTransformerContext context)
    {
        foreach (JsonPropertyInfo prop in context.JsonTypeInfo.Properties)
        {
            if (OptionalValue.IsOptionalValueType(prop.PropertyType))
            {
                var propAttributes = prop.AttributeProvider?.GetCustomAttributes(false) ?? [];

                // If the property has a [Specified] attribute, mark it as required
                if (propAttributes.Any(x => x.GetType().FullName == "OptionalValues.DataAnnotations.SpecifiedAttribute"))
                {
                    schema.Required ??= new HashSet<string>(StringComparer.Ordinal);
                    schema.Required.Add(prop.Name);
                }
            }
        }
    }

    private static bool IsSchemaReference(IOpenApiSchema? schema)
        => schema switch
        {
            OpenApiSchema actualSchema => actualSchema.Metadata?.TryGetValue("x-schema-id", out var schemaId) == true
                                          && !string.IsNullOrEmpty(schemaId as string),
            OpenApiSchemaReference => true,
            _ => false,
        };

    private static bool GetOptionalValueIsNullable(ICustomAttributeProvider? memberInfo)
    {
        NullabilityInfo? nullabilityInfo = GetOptionalValueNullabilityInfo(memberInfo);
        if (nullabilityInfo == null)
        {
            return false;
        }
        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }

    private static NullabilityInfo? GetNullabilityInfo(ICustomAttributeProvider memberInfo)
    {
        var nullabilityInfoContext = new NullabilityInfoContext();
        return memberInfo switch
        {
            PropertyInfo propertyInfo => nullabilityInfoContext.Create(propertyInfo),
            FieldInfo fieldInfo => nullabilityInfoContext.Create(fieldInfo),
            ParameterInfo parameterInfo => nullabilityInfoContext.Create(parameterInfo),
            _ => null,
        };
    }

    private static NullabilityInfo? GetOptionalValueNullabilityInfo(ICustomAttributeProvider? memberInfo)
    {
        if (memberInfo == null)
        {
            return null;
        }

        NullabilityInfo? nullabilityInfo = GetNullabilityInfo(memberInfo);
        if (nullabilityInfo == null)
        {
            return null;
        }

        if (OptionalValue.IsOptionalValueType(nullabilityInfo.Type))
        {
            return nullabilityInfo.GenericTypeArguments[0];
        }

        return null;
    }
}