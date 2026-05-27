using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace OptionalValues.Mvc;

/// <summary>
/// Provides MVC validation metadata for <see cref="OptionalValue{T}"/> so child validation is skipped.
/// </summary>
public sealed class OptionalValueValidationMetadataProvider : IValidationMetadataProvider
{
    /// <inheritdoc />
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!OptionalValue.IsOptionalValueType(context.Key.ModelType))
        {
            return;
        }

        context.ValidationMetadata.ValidateChildren = false;
    }
}
