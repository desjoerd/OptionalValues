using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OptionalValues.OpenApi.Tests;

public class OpenApiDocumentTestBase
{
    protected static async Task<OpenApiDocument> GetDocument()
    {
        await using var application = new WebApplicationFactory<Program>();
        IOpenApiDocumentProvider? documentProvider = application.Services.GetKeyedService<IOpenApiDocumentProvider>("v1");

        OpenApiDocument document = await documentProvider!.GetOpenApiDocumentAsync();
        return document;
    }
}