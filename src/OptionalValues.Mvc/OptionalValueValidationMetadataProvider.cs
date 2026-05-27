using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace OptionalValues.Mvc;

/// <summary>
/// Provides MVC validation metadata for <see cref="OptionalValue{T}"/>.
/// </summary>
internal sealed class OptionalValueValidationMetadataProvider : IValidationMetadataProvider
{
    /// <inheritdoc />
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Key.MetadataKind != ModelMetadataKind.Property ||
            context.Key.ContainerType is null ||
            !OptionalValue.IsOptionalValueType(context.Key.ContainerType))
        {
            return;
        }

        switch (context.Key.Name)
        {
            case nameof(OptionalValue<object>.Value):
                context.ValidationMetadata.ValidationModelName = string.Empty;
                break;
            default:
                context.ValidationMetadata.PropertyValidationFilter = NeverValidatePropertyFilter.Instance;
                break;
        }
    }
}
