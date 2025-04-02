using System.Text.Json.Serialization.Metadata;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;

namespace OptionalValues.OpenApi;

internal class ConfigureOpenApiOptions : IConfigureNamedOptions<OpenApiOptions>
{
    public void Configure(OpenApiOptions options)
    {
        options.AddSchemaTransformer<OptionalValueSchemaTransformer>();
    }

    public void Configure(string? name, OpenApiOptions options)
    {
        Func<JsonTypeInfo, string?> previousCreateSchemaReferenceId = options.CreateSchemaReferenceId;
        options.CreateSchemaReferenceId = jsonTypeInfo =>
        {
            if (OptionalValue.IsOptionalValueType(jsonTypeInfo.Type))
            {
                return null;
            }

            return previousCreateSchemaReferenceId(jsonTypeInfo);
        };

        options.AddSchemaTransformer<OptionalValueSchemaTransformer>();
    }
}