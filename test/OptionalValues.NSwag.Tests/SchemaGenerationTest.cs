using Namotion.Reflection;

using Newtonsoft.Json;

using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;

using NSwag;
using NSwag.Generation;

using Shouldly;

using Xunit.Abstractions;

namespace OptionalValues.NSwag.Tests;

public class SchemaGenerationTest(ITestOutputHelper output)
{
    [Theory]
    [InlineData(SchemaType.JsonSchema)]
    [InlineData(SchemaType.OpenApi3)]
    [InlineData(SchemaType.Swagger2)]
    public void SchemaGeneration_Should_Be_The_Same(SchemaType schemaType)
    {
        var schemaPlainJson = GetJsonSchemaAsString<ExamplesPlain.Primitives>(schemaType);
        var schemaOptionalValuesJson = GetJsonSchemaAsString<ExamplesOptionalValues.Primitives>(schemaType);

        output.WriteLine(schemaOptionalValuesJson);

        schemaOptionalValuesJson.ShouldNotBeNullOrEmpty();
        schemaOptionalValuesJson.ShouldBe(schemaPlainJson);
    }

    [Theory]
    [InlineData(SchemaType.JsonSchema)]
    [InlineData(SchemaType.OpenApi3)]
    [InlineData(SchemaType.Swagger2)]
    public void SchemaGeneration_Should_Be_The_Same_Ref(SchemaType schemaType)
    {
        var schemaPlainJson = GetJsonSchemaAsString<ExamplesPlain.Root>(schemaType);
        var schemaOptionalValuesJson = GetJsonSchemaAsString<ExamplesOptionalValues.Root>(schemaType);

        output.WriteLine(schemaOptionalValuesJson);

        schemaOptionalValuesJson.ShouldNotBeNullOrEmpty();
        schemaOptionalValuesJson.ShouldBe(schemaPlainJson);
    }

    [Fact]
    public void OpenApiSchemaGeneration_Should_Be_The_Same()
    {
        OpenApiDocument schemaPlainJson = GetOpenApiDocument<ExamplesPlain.Primitives>();
        OpenApiDocument schemaOptionalValuesJson = GetOpenApiDocument<ExamplesOptionalValues.Primitives>();

        var schemaPlainJsonString = GetOpenApiString(schemaPlainJson);
        var schemaOptionalValuesJsonString = GetOpenApiString(schemaOptionalValuesJson);

        schemaPlainJsonString.ShouldNotBeNullOrEmpty();

        schemaOptionalValuesJsonString.ShouldBe(schemaPlainJsonString);
    }

    [Fact]
    public void OpenApiSchemaGeneration_Should_Be_The_Same_Ref()
    {
        var schemaPlainJson = GetOpenApiDocument<ExamplesPlain.Root>();
        var schemaOptionalValuesJson = GetOpenApiDocument<ExamplesOptionalValues.Root>();

        var schemaPlainJsonString = GetOpenApiString(schemaPlainJson);
        var schemaOptionalValuesJsonString = GetOpenApiString(schemaOptionalValuesJson);

        schemaPlainJsonString.ShouldNotBeNullOrEmpty();

        schemaOptionalValuesJsonString.ShouldBe(schemaPlainJsonString);
    }


    private string GetJsonSchemaAsString<T>(SchemaType schemaType)
    {
        JsonSchemaGeneratorSettings settings = new SystemTextJsonSchemaGeneratorSettings
        {
            SchemaType = schemaType,
            DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
        }.AddOptionalValueSupport();

        var jsonSchemaGenerator = new JsonSchemaGenerator(settings);

        var jsonSchema = new JsonSchema();
        var jsonSchemaResolver = new JsonSchemaResolver(jsonSchema, settings);

        jsonSchemaGenerator.Generate(jsonSchema, typeof(T), jsonSchemaResolver);

        var schemaJson = jsonSchema.ToJson();
        return schemaJson;
    }

    private OpenApiDocument GetOpenApiDocument<T>()
    {
        var openApiDocumentGeneratorSettings = new OpenApiDocumentGeneratorSettings();
        openApiDocumentGeneratorSettings.SchemaSettings.AddOptionalValueSupport();
        openApiDocumentGeneratorSettings.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;

        OpenApiSchemaGenerator? openApiSchemaGenerator = openApiDocumentGeneratorSettings.SchemaGeneratorFactory();

        var document = new OpenApiDocument();
        var jsonSchemaResolver = new OpenApiSchemaResolver(document, openApiDocumentGeneratorSettings.SchemaSettings);

        openApiSchemaGenerator.Generate(typeof(T).ToContextualType(), jsonSchemaResolver);

        return document;
    }

    private static string GetOpenApiString(OpenApiDocument openApiDocument)
        => JsonSchemaSerialization.ToJson(openApiDocument, SchemaType.OpenApi3, OpenApiDocument.GetJsonSerializerContractResolver(SchemaType.OpenApi3), Formatting.Indented);


    private static class ExamplesOptionalValues
    {
        public class Primitives
        {
            public OptionalValue<int> IntValue { get; set; }
            public OptionalValue<int?> NullableIntValue { get; set; }
            public OptionalValue<string> StringValue { get; set; }
            public OptionalValue<string?> NullableStringValue { get; set; }
            public OptionalValue<bool> BoolValue { get; set; }
            public OptionalValue<bool?> NullableBoolValue { get; set; }
            public OptionalValue<Guid> GuidValue { get; set; }
            public OptionalValue<Guid?> NullableGuidValue { get; set; }
            public OptionalValue<TestEnum> EnumValue { get; set; }
            public OptionalValue<TestEnum?> NullableEnumValue { get; set; }
        }

        public class Root
        {
            public OptionalValue<Primitives> Primitives { get; set; } = null!;
            public OptionalValue<Primitives?> NullablePrimitives { get; set; }
        }
    }

    private static class ExamplesPlain
    {
        public class Primitives
        {
            public int IntValue { get; set; }
            public int? NullableIntValue { get; set; }
            public string StringValue { get; set; } = null!;
            public string? NullableStringValue { get; set; }
            public bool BoolValue { get; set; }
            public bool? NullableBoolValue { get; set; }
            public Guid GuidValue { get; set; }
            public Guid? NullableGuidValue { get; set; }
            public TestEnum EnumValue { get; set; }
            public TestEnum? NullableEnumValue { get; set; }
        }

        public class Root
        {
            public Primitives Primitives { get; set; } = null!;
            public Primitives? NullablePrimitives { get; set; }
        }
    }

    private enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }
}