using NJsonSchema;
using NJsonSchema.Generation;

using Shouldly;

namespace OptionalValues.NSwag.Tests;

public class SchemaGenerationTest
{
    private static readonly JsonSchemaGeneratorSettings Settings = new SystemTextJsonSchemaGeneratorSettings
    {
        TypeMappers =
        {
            new OptionalValueTypeMapper(),
        },
    };

    [Fact]
    public void SchemaGeneration_Should_Be_The_Same()
    {
        var jsonSchemaGenerator = new JsonSchemaGenerator(Settings);
        var jsonSchemaResolver = new JsonSchemaResolver(new JsonSchema(), Settings);

        JsonSchema schemaOptionalValues = jsonSchemaGenerator.Generate(typeof(ExamplesOptionalValues.Primitives), jsonSchemaResolver);
        JsonSchema schemaPlain = jsonSchemaGenerator.Generate(typeof(ExamplesPlain.Primitives), jsonSchemaResolver);

        var schemaOptionalValuesJson = schemaOptionalValues.ToJson();
        var schemaPlainJson = schemaPlain.ToJson();

        schemaOptionalValuesJson.ShouldNotBeNullOrEmpty();
        schemaOptionalValuesJson.ShouldBe(schemaPlainJson);
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