using Microsoft.OpenApi;

namespace OptionalValues.OpenApi.Tests;

public class PrimitivesTest : OpenApiDocumentTestBase
{
    [Fact]
    public async Task Body()
    {
        OpenApiDocument document = await GetDocument();
        (OpenApiSchema baselineSchema, OpenApiSchema optionalSchema) = document.GetComparisonOperationRequestBodySchemasByPath("/primitives/body");
        await optionalSchema.ShouldBeEqualToBaselineSchema(baselineSchema);
    }
}