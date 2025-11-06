using Microsoft.OpenApi;

using Shouldly;

namespace OptionalValues.OpenApi.Tests;

public static class SchemaAssertionExtensions
{
    public static async Task ShouldBeEqualToBaselineSchema(this OpenApiSchema actualSchema, OpenApiSchema baselineSchema, params string[] ignoreStrings)
    {
        var actualSchemaJson = await actualSchema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
        var baselineSchemaJson = await baselineSchema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

        foreach(var ignoreString in ignoreStrings)
        {
            actualSchemaJson = actualSchemaJson.Replace(ignoreString, "");
            baselineSchemaJson = baselineSchemaJson.Replace(ignoreString, "");
        }

        actualSchemaJson.ShouldBe(baselineSchemaJson);
    }
}