using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

using Shouldly;

namespace OptionalValues.OpenApi.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        await using var application = new WebApplicationFactory<Program>();
        IOpenApiDocumentProvider? documentProvider = application.Services.GetKeyedService<IOpenApiDocumentProvider>("v1");

        OpenApiDocument document = await documentProvider!.GetOpenApiDocumentAsync();

        document.ShouldNotBeNull();
    }
}