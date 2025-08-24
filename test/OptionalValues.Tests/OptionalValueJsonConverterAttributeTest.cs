using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptionalValues.Tests;

public class OptionalValueJsonConverterAttributeTest
{
    public class UpperCaseStringJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString()?.ToUpperInvariant();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUpperInvariant());
        }
    }

    public class ExampleModel
    {
        /// <summary>
        /// JsonStringEnumConverter which is a JsonConverterFactory
        /// </summary>
        [OptionalValueJsonConverter(typeof(JsonStringEnumConverter<ExampleEnum>))]
        public OptionalValue<ExampleEnum> EnumValue { get; set; }

        /// <summary>
        /// Normal converter
        /// </summary>
        [OptionalValueJsonConverter(typeof(UpperCaseStringJsonConverter))]
        public OptionalValue<string> UpperStringValue { get; set; }
    }

    public enum ExampleEnum
    {
        Foo,
        Bar
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        .AddOptionalValueSupport();

    [Fact]
    public void ShouldSerializeWithCustomConverterOnProperty()
    {
        var model = new ExampleModel
        {
            EnumValue = new OptionalValue<ExampleEnum>(ExampleEnum.Foo),
            UpperStringValue = new OptionalValue<string>("value"),
        };

        var json = JsonSerializer.Serialize(model, JsonSerializerOptions);
        Assert.Equal("""{"EnumValue":"Foo","UpperStringValue":"VALUE"}""", json);
    }

    [Fact]
    public void ShouldDeserializeWithCustomConverterOnProperty()
    {
        var json = """{"EnumValue":"Bar","UpperStringValue":"value"}""";
        ExampleModel model = JsonSerializer.Deserialize<ExampleModel>(json, JsonSerializerOptions)!;

        Assert.Equal(ExampleEnum.Bar, model.EnumValue.Value);
        Assert.Equal("VALUE", model.UpperStringValue.Value);
    }
}

[JsonSerializable(typeof(OptionalValueJsonConverterAttributeTest.ExampleModel))]
[JsonSerializable(typeof(OptionalValueJsonConverterAttributeTest.ExampleEnum))]
[JsonSerializable(typeof(string))]
public partial class OptionalValueJsonConverterAttributeTestWithSourceGeneratorJsonSerializationContext : JsonSerializerContext
{
}