using Microsoft.OpenApi;

namespace OptionalValues.OpenApi.Tests;

public class ReferencesTest : OpenApiDocumentTestBase
{
    [Fact]
    public async Task Body()
    {
        OpenApiDocument document = await GetDocument();
        (OpenApiSchema baselineSchema, OpenApiSchema optionalSchema) = document.GetComparisonOperationRequestBodySchemasByPath("/references/body");
        await optionalSchema.ShouldBeEqualToBaselineSchema(baselineSchema, "Optional", "Baseline");
    }
}

