using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptionalValues.Tests;

public class OptionalValueJsonConverterAttributeTest
{
    public class ExampleModel
    {
        [OptionalValueJsonConverter(typeof(JsonStringEnumConverter<ExampleEnum>))]
        public OptionalValue<ExampleEnum> EnumValue { get; set; }
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
            EnumValue = new OptionalValue<ExampleEnum>(ExampleEnum.Foo)
        };

        var json = JsonSerializer.Serialize(model, JsonSerializerOptions);
        Assert.Equal("""{"EnumValue":"Foo"}""", json);
    }

    [Fact]
    public void ShouldDeserializeWithCustomConverterOnProperty()
    {
        var json = """{"EnumValue":"Bar"}""";
        ExampleModel model = JsonSerializer.Deserialize<ExampleModel>(json, JsonSerializerOptions)!;

        Assert.Equal(ExampleEnum.Bar, model.EnumValue.Value);
    }
}