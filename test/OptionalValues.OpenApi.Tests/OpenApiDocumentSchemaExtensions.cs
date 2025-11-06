using Microsoft.OpenApi;

namespace OptionalValues.OpenApi.Tests;

public static class OpenApiDocumentSchemaExtensions
{
    public static (OpenApiSchema baseline, OpenApiSchema optional) GetComparisonOperationRequestBodySchemasByPath(this OpenApiDocument document, string pathBase)
    {
        IOpenApiSchema? baselineRequestBodySchema = document
            .Paths[$"{pathBase}/baseline"]
            .Operations![HttpMethod.Post]
            .RequestBody!
            .Content!["application/json"]
            .Schema;

        IOpenApiSchema? optionalRequestBodySchema = document
            .Paths[$"{pathBase}/optional"]
            .Operations![HttpMethod.Post]
            .RequestBody!
            .Content!["application/json"]
            .Schema;

        return (baselineRequestBodySchema!.Unwrap(), optionalRequestBodySchema!.Unwrap());
    }

    private static OpenApiSchema Unwrap(this IOpenApiSchema schema)
    {
        if (schema is OpenApiSchema actualSchema)
        {
            return actualSchema;
        }

        if (schema is OpenApiSchemaReference reference)
        {
            return reference.RecursiveTarget ?? throw new InvalidOperationException("Schema reference target is null.");
        }

        throw new InvalidOperationException("Schema is neither a schema nor a reference.");
    }
}