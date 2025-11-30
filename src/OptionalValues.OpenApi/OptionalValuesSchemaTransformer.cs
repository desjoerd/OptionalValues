using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        // Get the OpenApiOptions of the current document
        IOptionsMonitor<OpenApiOptions> openApiOptionsSnapshot = context.ApplicationServices.GetRequiredService<IOptionsMonitor<OpenApiOptions>>();
        OpenApiOptions openApiOptions = openApiOptionsSnapshot.Get(context.DocumentName);

        Type underlyingType = OptionalValue.GetUnderlyingType(context.JsonPropertyInfo.PropertyType);
        var underlyingSchemaId = openApiOptions.CreateSchemaReferenceId(
            JsonTypeInfo.CreateJsonTypeInfo(underlyingType, context.JsonTypeInfo.Options));
        var isSchemaReference = !string.IsNullOrEmpty(underlyingSchemaId);

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
        schema.Pattern = underlyingSchema.Pattern;

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
        var customAttributes = context.JsonPropertyInfo.AttributeProvider?.GetCustomAttributes(false) ?? [];
        var isNullable = !customAttributes.OfType<RequiredAttribute>().Any()
                         && GetOptionalValueIsNullable(context.JsonPropertyInfo.AttributeProvider as MemberInfo);
        if (isNullable)
        {
            if (!isSchemaReference)
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
                if (propAttributes.Any(x => x.GetType()?.FullName == "OptionalValues.DataAnnotations.SpecifiedAttribute"))
                {
                    schema.Required ??= new HashSet<string>(StringComparer.Ordinal);
                    schema.Required.Add(prop.Name);
                }
            }
        }
    }

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
        if (nullabilityInfo == null
            || !OptionalValue.IsOptionalValueType(nullabilityInfo.Type))
        {
            return null;
        }

        return nullabilityInfo.GenericTypeArguments[0];
    }
}