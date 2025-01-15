using System.Text.Json;

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

using Shouldly;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace OptionalValues.Swashbuckle.Tests;

public class SchemaGeneratorTest
{
    private SchemaGenerator SchemaGeneratorDefault { get; } = new SchemaGenerator(new SchemaGeneratorOptions
    {
        // This should be true, because Nullable Reference Support is built into the OptionalValue<T> type
        // So we set it to true to match the behavior of the OptionalValueDataContractResolver
        SupportNonNullableReferenceTypes = true,
    }, new JsonSerializerDataContractResolver(JsonSerializerOptions.Default));

    private SchemaGenerator SchemaGeneratorOptionalValues { get; } = new SchemaGenerator(new SchemaGeneratorOptions
    {
        SupportNonNullableReferenceTypes = true,
    }, new OptionalValueDataContractResolver(new JsonSerializerDataContractResolver(JsonSerializerOptions.Default)));

    [Fact]
    public void Should_Generate_The_Same_Schema_With_OptionalValues()
    {
        var schemaRepositoryForOptionalValues = new SchemaRepository();
        OpenApiSchema? schemaOptionalValuesAsRef = SchemaGeneratorOptionalValues.GenerateSchema(typeof(ExamplesOptionalValues.Primitives), schemaRepositoryForOptionalValues);

        var schemaRepositoryForDefault = new SchemaRepository();
        OpenApiSchema? schemaDefaultAsRef = SchemaGeneratorDefault.GenerateSchema(typeof(ExamplesPlain.Primitives), schemaRepositoryForDefault);

        OpenApiSchema? schemaOptionalValues = schemaRepositoryForOptionalValues.Schemas[schemaOptionalValuesAsRef.Reference.Id];
        OpenApiSchema? schemaDefault = schemaRepositoryForDefault.Schemas[schemaDefaultAsRef.Reference.Id];

        var schemaOptionalValuesJson = SerializeSchema(schemaOptionalValues);
        var schemaDefaultJson = SerializeSchema(schemaDefault);

        schemaOptionalValuesJson.ShouldBe(schemaDefaultJson);
        schemaOptionalValues.Properties.Count.ShouldBe(4);
        schemaDefault.Properties.Count.ShouldBe(4);
    }

    [Fact]
    public void OptionalValue_Support_Should_Not_Change_Behavior()
    {
        var schemaRepositoryForOptionalValues = new SchemaRepository();
        OpenApiSchema? schemaOptionalValuesAsRef = SchemaGeneratorOptionalValues.GenerateSchema(typeof(ExamplesPlain.Primitives), schemaRepositoryForOptionalValues);

        var schemaRepositoryForDefault = new SchemaRepository();
        OpenApiSchema? schemaDefault = SchemaGeneratorDefault.GenerateSchema(typeof(ExamplesPlain.Primitives), schemaRepositoryForDefault);

        OpenApiSchema? schema1 = schemaRepositoryForOptionalValues.Schemas[schemaOptionalValuesAsRef.Reference.Id];
        OpenApiSchema? schema2 = schemaRepositoryForDefault.Schemas[schemaDefault.Reference.Id];

        var schema1Json = SerializeSchema(schema1);
        var schema2Json = SerializeSchema(schema2);

        schema1Json.ShouldBe(schema2Json);

        schema1.Properties.Count.ShouldBe(4);
        schema2.Properties.Count.ShouldBe(4);
    }

    private static string SerializeSchema(OpenApiSchema schema)
    {
        using var textWriter = new StringWriter();
        var openApiWriter = new OpenApiJsonWriter(textWriter);
        schema.SerializeAsV3(openApiWriter);
        var json = textWriter.ToString();

        json.ShouldNotBeNullOrEmpty();

        return json;
    }

    private static class ExamplesOptionalValues
    {
        public class Primitives
        {
            public OptionalValue<int> IntValue { get; set; }
            public OptionalValue<string> StringValue { get; set; }
            public OptionalValue<bool> BoolValue { get; set; }
            public OptionalValue<Guid> GuidValue { get; set; }
        }
    }

    private static class ExamplesPlain
    {
        public class Primitives
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; } = null!;
            public bool BoolValue { get; set; }
            public Guid GuidValue { get; set; }
        }
    }
}
