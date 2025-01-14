using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using OptionalValues.Internal;

namespace OptionalValues;

internal static class OptionalValueJsonTypeInfoResolverModifier
{
    internal static void ModifyTypeInfo(JsonTypeInfo jsonTypeInfo)
    {
        foreach (JsonPropertyInfo jsonPropertyInfo in jsonTypeInfo.Properties)
        {
            if (!OptionalValue.IsOptionalValueType(jsonPropertyInfo.PropertyType))
            {
                continue;
            }

            jsonPropertyInfo.ShouldSerialize = (_, value) => ((IOptionalValueInternals)value!).IsSpecified;
            jsonPropertyInfo.IsRequired = false; // OptionalValue<T> is never required

#if NET9_0_OR_GREATER
            if (jsonTypeInfo.Options.RespectNullableAnnotations && !jsonTypeInfo.Type.IsGenericType)
            {
                TryToRespectNullableAnnotationsForProperty(jsonPropertyInfo);
            }
#endif
        }
    }

#if NET9_0_OR_GREATER
    private static void TryToRespectNullableAnnotationsForProperty(JsonPropertyInfo jsonPropertyInfo)
    {
        if (OptionalValue.GetUnderlyingType(jsonPropertyInfo.PropertyType).IsValueType)
        {
            return;
        }

        NullabilityInfo? nullabilityInfo = CreateNullabilityInfo(jsonPropertyInfo.AttributeProvider);
        if (nullabilityInfo is null)
        {
            return;
        }

        if (nullabilityInfo.WriteState == NullabilityState.NotNull)
        {
            AddNotNullableValidation(jsonPropertyInfo);
        }
    }

    private static NullabilityInfo? CreateNullabilityInfo(ICustomAttributeProvider? member)
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

    private static void AddNotNullableValidation(JsonPropertyInfo jsonPropertyInfo)
    {
        jsonPropertyInfo.IsSetNullable = false;
        Action<object, object?>? originalSet = jsonPropertyInfo.Set;
        jsonPropertyInfo.Set = (target, value) =>
        {
            if (value!.Equals(null))
            {
                throw new JsonException($"Property '{jsonPropertyInfo.Name}' is not nullable.");
            }

            originalSet?.Invoke(target, value);
        };
    }
#endif
}