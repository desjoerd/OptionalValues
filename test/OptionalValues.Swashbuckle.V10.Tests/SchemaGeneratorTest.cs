using System.Text.Json;

using Microsoft.OpenApi;

using Shouldly;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace OptionalValues.Swashbuckle.V10.Tests;

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
        // This should be true, because Nullable Reference Support is built into the OptionalValue<T> type
        // So we set it to true to match the behavior of the OptionalValueDataContractResolver
        SupportNonNullableReferenceTypes = true,
    }, new OptionalValueDataContractResolver(new JsonSerializerDataContractResolver(JsonSerializerOptions.Default)));

    [Fact]
    public void Should_Generate_The_Same_Schema_With_OptionalValues()
    {
        var schemaRepositoryForOptionalValues = new SchemaRepository();
        var schemaRepositoryForDefault = new SchemaRepository();

        IOpenApiSchema schemaOptionalValuesAsRef = SchemaGeneratorOptionalValues.GenerateSchema(typeof(ExamplesOptionalValues.Primitives), schemaRepositoryForOptionalValues);
        schemaOptionalValuesAsRef.ShouldNotBeNull();

        IOpenApiSchema schemaDefaultAsRef = SchemaGeneratorDefault.GenerateSchema(typeof(ExamplesPlain.Primitives), schemaRepositoryForDefault);
        schemaDefaultAsRef.ShouldNotBeNull();

        var refId1 = ((OpenApiSchemaReference)schemaOptionalValuesAsRef).Reference.Id;
        var refId2 = ((OpenApiSchemaReference)schemaDefaultAsRef).Reference.Id;

        IOpenApiSchema schemaOptionalValues = schemaRepositoryForOptionalValues.Schemas[refId1!];
        IOpenApiSchema schemaDefault = schemaRepositoryForDefault.Schemas[refId2!];

        var schemaOptionalValuesJson = SerializeSchema(schemaOptionalValues);
        var schemaDefaultJson = SerializeSchema(schemaDefault);

        schemaOptionalValuesJson.ShouldBe(schemaDefaultJson);
        schemaOptionalValues.Properties?.Count.ShouldBe(4);
        schemaDefault.Properties?.Count.ShouldBe(4);
    }

    [Fact]
    public void OptionalValue_Support_Should_Not_Change_Behavior()
    {
        var schemaRepositoryForOptionalValues = new SchemaRepository();
        var schemaRepositoryForDefault = new SchemaRepository();

        IOpenApiSchema schemaOptionalValuesAsRef = SchemaGeneratorOptionalValues.GenerateSchema(typeof(ExamplesPlain.Primitives), schemaRepositoryForOptionalValues);
        schemaOptionalValuesAsRef.ShouldNotBeNull();

        IOpenApiSchema schemaDefaultAsRef = SchemaGeneratorDefault.GenerateSchema(typeof(ExamplesPlain.Primitives), schemaRepositoryForDefault);
        schemaDefaultAsRef.ShouldNotBeNull();

        var refId1 = ((OpenApiSchemaReference)schemaOptionalValuesAsRef).Reference.Id;
        var refId2 = ((OpenApiSchemaReference)schemaDefaultAsRef).Reference.Id;

        IOpenApiSchema schema1 = schemaRepositoryForOptionalValues.Schemas[refId1!];
        IOpenApiSchema schema2 = schemaRepositoryForDefault.Schemas[refId2!];

        var schema1Json = SerializeSchema(schema1);
        var schema2Json = SerializeSchema(schema2);

        schema1Json.ShouldBe(schema2Json);
    }

    private static string SerializeSchema(Microsoft.OpenApi.IOpenApiSchema schema)
    {
        using var stringWriter = new StringWriter();
        // OpenApi 2.x uses OpenApiWriterBase instead of OpenApiJsonWriter
        var writer = new Microsoft.OpenApi.OpenApiJsonWriter(stringWriter);
        schema.SerializeAsV31(writer);
        return stringWriter.ToString();
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
