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

            if (!ShouldSerializeCache.TryGetValue(jsonPropertyInfo.PropertyType, out Func<object, object?, bool>? shouldSerialize))
            {
                PropertyInfo? property = jsonPropertyInfo.PropertyType.GetProperty(nameof(OptionalValue<object>.IsSpecified));
                shouldSerialize = CreateShouldSerializeBasedOnBoolProperty(property!);
                ShouldSerializeCache.TryAdd(jsonPropertyInfo.PropertyType, shouldSerialize);
            }

            jsonPropertyInfo.ShouldSerialize = shouldSerialize;
        }
    }

    private static Func<object, object?, bool> CreateShouldSerializeBasedOnBoolProperty(PropertyInfo propertyInfo)
    {
        ParameterExpression discard = Expression.Parameter(typeof(object), "discard");
        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
        UnaryExpression convertedInstance = Expression.Convert(instance, propertyInfo.DeclaringType!);
        MemberExpression propertyAccess = Expression.Property(convertedInstance, propertyInfo);

        var lambda = Expression.Lambda<Func<object, object?, bool>>(propertyAccess, discard, instance);
        return lambda.Compile();
    }
}