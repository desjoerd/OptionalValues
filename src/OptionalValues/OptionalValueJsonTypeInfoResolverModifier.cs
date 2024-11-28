using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
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

            Func<object, object?, bool> shouldSerialize = ShouldSerializeCache.GetOrAdd(jsonPropertyInfo.PropertyType, CreateShouldSerializeForOptionalValueTypeBasedOnIsDefined);
            jsonPropertyInfo.ShouldSerialize = shouldSerialize;
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
}