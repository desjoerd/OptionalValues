using System.Text.Json;

using FluentAssertions;

using Microsoft.OpenApi.Models;

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

        schemaOptionalValues.Should().BeEquivalentTo(schemaDefault);
        schemaOptionalValues.Properties.Should().HaveCount(4);
        schemaDefault.Properties.Should().HaveCount(4);
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

        schema1.Should().BeEquivalentTo(schema2);
        schema1.Properties.Should().HaveCount(4);
        schema2.Properties.Should().HaveCount(4);
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