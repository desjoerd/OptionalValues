using Microsoft.AspNetCore.Mvc;

namespace OptionalValues.Mvc;

/// <summary>
/// Extension methods for <see cref="MvcOptions"/> to add support for <see cref="OptionalValue{T}"/> validation metadata.
/// </summary>
public static class MvcOptionsExtensions
{
    /// <summary>
    /// Adds validation metadata support for <see cref="OptionalValue{T}"/> so MVC does not validate its child properties.
    /// </summary>
    /// <param name="options">The MVC options to configure.</param>
    public static void AddOptionalValuesMvc(this MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.ModelMetadataDetailsProviders.OfType<OptionalValueValidationMetadataProvider>().Any())
        {
            return;
        }

        options.ModelMetadataDetailsProviders.Add(new OptionalValueValidationMetadataProvider());
    }
}
