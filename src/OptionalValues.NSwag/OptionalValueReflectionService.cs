using System.Diagnostics;

using Namotion.Reflection;

using NJsonSchema.Generation;

namespace OptionalValues.NSwag;

/// <inheritdoc />
public class OptionalValueReflectionService : SystemTextJsonReflectionService
{
    /// <inheritdoc />
    public override bool IsNullable(ContextualType contextualType, ReferenceTypeNullHandling defaultReferenceTypeNullHandling)
    {
        Debug.Assert(contextualType != null, nameof(contextualType) + " != null");

        if (!OptionalValue.IsOptionalValueType(contextualType.Type))
        {
            return base.IsNullable(contextualType, defaultReferenceTypeNullHandling);
        }

        var underlyingIsNullable = contextualType.GenericArguments[0].IsNullableType
                                   || contextualType.GenericArguments[0].Nullability == Nullability.Nullable;

        return underlyingIsNullable;
    }
}