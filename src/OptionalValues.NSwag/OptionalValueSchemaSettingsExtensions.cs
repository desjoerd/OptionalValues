using NJsonSchema.Generation;

namespace OptionalValues.NSwag;

/// <summary>
/// Registration extensions for JsonSchemaGeneratorSettings.
/// </summary>
public static class OptionalValueSchemaSettingsExtensions
{
    /// <summary>
    /// Adds support for <see cref="OptionalValue{T}"/> to the <see cref="JsonSchemaGeneratorSettings"/>.
    /// </summary>
    /// <param name="schemaGeneratorSettings"></param>
    /// <remarks>The <see cref="JsonSchemaGeneratorSettings"/> must be of type <see cref="SystemTextJsonSchemaGeneratorSettings"/>.</remarks>
    /// <returns>The <see cref="SystemTextJsonSchemaGeneratorSettings"/> for easy chaining.</returns>
    /// <exception cref="ArgumentException">When the <see cref="JsonSchemaGeneratorSettings"/> is not of type <see cref="SystemTextJsonSchemaGeneratorSettings"/>.</exception>
    public static SystemTextJsonSchemaGeneratorSettings AddOptionalValueSupport(this JsonSchemaGeneratorSettings schemaGeneratorSettings)
    {
        ArgumentNullException.ThrowIfNull(schemaGeneratorSettings);

        if (schemaGeneratorSettings is not SystemTextJsonSchemaGeneratorSettings settings)
        {
            throw new ArgumentException("The schema generator settings must be of type SystemTextJsonSchemaGeneratorSettings.");
        }

        settings.TypeMappers.Add(new OptionalValueTypeMapper());
        settings.ReflectionService = new OptionalValueReflectionService();

        return settings;
    }
}
