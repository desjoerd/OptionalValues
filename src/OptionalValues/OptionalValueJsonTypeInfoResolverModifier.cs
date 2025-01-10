using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace OptionalValues;

internal static class OptionalValueJsonTypeInfoResolverModifier
{
    private static readonly ConcurrentDictionary<Type, Func<object, object?, bool>> ShouldSerializeCache = new();

    internal static void ModifyTypeInfo(JsonTypeInfo jsonTypeInfo)
    {
        foreach (JsonPropertyInfo jsonPropertyInfo in jsonTypeInfo.Properties)
        {
            if (!OptionalValue.IsOptionalValueType(jsonPropertyInfo.PropertyType))
            {
                continue;
            }

            Func<object, object?, bool> shouldSerialize = ShouldSerializeCache.GetOrAdd(
                jsonPropertyInfo.PropertyType,
                CreateShouldSerializeForOptionalValueTypeBasedOnIsDefined);

            jsonPropertyInfo.ShouldSerialize = shouldSerialize;

#if NET9_0_OR_GREATER
            if (jsonTypeInfo.Options.RespectNullableAnnotations && !jsonTypeInfo.Type.IsGenericType)
            {
                TryToRespectNullableAnnotationsForProperty(jsonPropertyInfo);
            }
#endif
        }
    }

    private static Func<object, object?, bool> CreateShouldSerializeForOptionalValueTypeBasedOnIsDefined(Type optionalValueType)
    {
        PropertyInfo isSpecifiedProperty = optionalValueType.GetProperty(nameof(OptionalValue<object>.IsSpecified))!;

        ParameterExpression discard = Expression.Parameter(typeof(object), "discard");
        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
        UnaryExpression convertedInstance = Expression.Convert(instance, isSpecifiedProperty.DeclaringType!);
        MemberExpression propertyAccess = Expression.Property(convertedInstance, isSpecifiedProperty);

        // If optionalValueType is OptionalValue<string> then the result would be:
        // (object discard, object instance) => ((OptionalValue<string>)instance).IsSpecified;

        var lambda = Expression.Lambda<Func<object, object?, bool>>(propertyAccess, discard, instance);
        return lambda.Compile();
    }

#if NET9_0_OR_GREATER
    private static void TryToRespectNullableAnnotationsForProperty(JsonPropertyInfo jsonPropertyInfo)
    {
        if (OptionalValue.GetUnderlyingType(jsonPropertyInfo.PropertyType).IsValueType)
        {
            return;
        }

        var customAttributes = jsonPropertyInfo.AttributeProvider?
            .GetCustomAttributes(typeof(NullableAttribute), false) ?? [];

        if (customAttributes.Length == 0)
        {
            return;
        }

        var nullableAttribute = (NullableAttribute)customAttributes[0];

        var flags = nullableAttribute.NullableFlags;
        if (flags.Length >= 2 && flags[1] == 1)
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
    }
#endif
}