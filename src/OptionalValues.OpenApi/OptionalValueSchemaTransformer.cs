using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace OptionalValues.OpenApi;

public class OptionalValueSchemaTransformer(
    IOptionsSnapshot<OpenApiOptions> openApiOptionsSnapshot,
    IOptionsSnapshot<JsonOptions> httpJsonOptionsSnapshot) : IOpenApiSchemaTransformer
{

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine(context.JsonTypeInfo.Type);
        if (OptionalValue.IsOptionalValueType(context.JsonTypeInfo.Type))
        {
            // skip as these are going to be omitted
            // we will add the underlying type instead
            // schema.Type = "object";

            return Task.CompletedTask;
        }

        OpenApiOptions openApiOptions = openApiOptionsSnapshot.Get(context.DocumentName);
        JsonOptions httpJsonOptions = httpJsonOptionsSnapshot.Value;

        foreach(JsonPropertyInfo property in context.JsonTypeInfo.Properties)
        {
            if (!OptionalValue.IsOptionalValueType(property.PropertyType))
            {
                // we are only interested in OptionalValue<T> properties
                continue;
            }

            Type optionalValueUnderlyingType = OptionalValue.GetUnderlyingType(property.PropertyType);
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(optionalValueUnderlyingType) ?? optionalValueUnderlyingType;

            var isNullable = optionalValueUnderlyingType != nullableUnderlyingType;
            if (!isNullable)
            {
                // Check if it's a nullable reference type
                NullabilityInfo? nullableReferenceInfo = CreateNullabilityInfo(property.AttributeProvider);
                isNullable = nullableReferenceInfo?.WriteState == NullabilityState.Nullable;
            }

            if (SimpleTypeSchemas.TryGetSimpleTypeSchema(nullableUnderlyingType, out OpenApiSchema? simpleTypeSchema))
            {
                var propertySchema = new OpenApiSchema(simpleTypeSchema)
                {
                    Nullable = isNullable,
                };

                if (property.AttributeProvider?.GetCustomAttributes(false).OfType<ValidationAttribute>() is { } validationAttributes)
                {
                    propertySchema.ApplyValidationAttributes(validationAttributes);
                }

                schema.Properties[property.Name] = propertySchema;
            }
            else
            {
                schema.Properties[property.Name] = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = openApiOptions.CreateSchemaReferenceId(JsonTypeInfo.CreateJsonTypeInfo(nullableUnderlyingType, httpJsonOptions.SerializerOptions))
                    }
                    // Annotations = new Dictionary<string, object?>
                    // {
                    //     ["x-schema-id"] = openApiOptions.CreateSchemaReferenceId(JsonTypeInfo.CreateJsonTypeInfo(property.PropertyType, httpJsonOptions.SerializerOptions))
                    // },

                };
            }

            // OptionalValue<T> is never required
            schema.Required.Remove(property.Name);
        }

        return Task.CompletedTask;
    }

    static NullabilityInfo? CreateNullabilityInfo(ICustomAttributeProvider? member)
    {
        var nullability = new NullabilityInfoContext();
        return member switch
        {
            PropertyInfo propertyInfo => nullability.Create(propertyInfo).GenericTypeArguments[0],
            FieldInfo fieldInfo => nullability.Create(fieldInfo).GenericTypeArguments[0],
            ParameterInfo parameterInfo => nullability.Create(parameterInfo).GenericTypeArguments[0],
            _ => null,
        };
    }
}