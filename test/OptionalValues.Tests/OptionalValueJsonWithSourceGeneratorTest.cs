using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptionalValues.Tests;

public class OptionalValueJsonWithSourceGeneratorTest
{
    private static JsonSerializerOptions CreateOptionsSingleContext()
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = OptionalValueJsonWithSourceGeneratorJsonSerializationContext.Default
        };
        options.AddOptionalValueSupport();
        return options;
    }

    private static JsonSerializerOptions CreateOptionsMultipleContexts()
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolverChain =
            {
                OptionalValueJsonWithSourceGeneratorJsonSerializationContext.Default,
                OtherOptionalValueJsonWithSourceGeneratorJsonSerializationContext.Default
            }
        };
        // This needs to be done last, because it will add modifiers to all resolvers in the chain.
        options.AddOptionalValueSupport();
        return options;
    }

    [Fact]
    public void SerializeWithValues_ShouldWriteValues()
    {
        var model = new TestModel
        {
            Name = new OptionalValue<string>("John"),
            Age = new OptionalValue<int>(42)
        };

        var options = CreateOptionsSingleContext();
        var json = JsonSerializer.Serialize(model, options);

        Assert.Equal("""{"Name":"John","Age":42}""", json);
    }

    [Fact]
    public void SerializeWithUnspecified_ShouldNotWriteValues()
    {
        var model = new TestModel
        {
            Name = OptionalValue<string>.Unspecified,
            Age = OptionalValue<int>.Unspecified
        };

        var options = CreateOptionsSingleContext();
        var json = JsonSerializer.Serialize(model, options);

        Assert.Equal("{}", json);
    }

    [Fact]
    public void DeserializeWithValues_ShouldReadValues()
    {
        var json = """{"Name":"John","Age":42}""";

        var options = CreateOptionsSingleContext();
        var model = JsonSerializer.Deserialize<TestModel>(json, options)!;

        Assert.Equal("John", model.Name.Value);
        Assert.Equal(42, model.Age.Value);
    }

    [Fact]
    public void DeserializeWithUnspecified_ShouldReadUnspecified()
    {
        var json = "{}";

        var options = CreateOptionsSingleContext();
        var model = JsonSerializer.Deserialize<TestModel>(json, options)!;

        Assert.False(model.Name.IsSpecified);
        Assert.False(model.Age.IsSpecified);
    }

    [Fact]
    public void SerializeWithValuesMultipleContexts_ShouldWriteValues()
    {
        var model = new TestModelOtherContext
        {
            Test = new TestModel
            {
                Name = new OptionalValue<string>("John"),
                Age = new OptionalValue<int>(42)
            },
            Street = new OptionalValue<string>("Main Street"),
            HouseNumber = new OptionalValue<int>(42)
        };

        var options = CreateOptionsMultipleContexts();
        var json = JsonSerializer.Serialize(model, options);

        Assert.Equal("""{"Test":{"Name":"John","Age":42},"Street":"Main Street","HouseNumber":42}""", json);
    }

    [Fact]
    public void SerializeWithUnspecifiedMultipleContexts_ShouldNotWriteValues()
    {
        var model = new TestModelOtherContext
        {
            Test = new TestModel
            {
                Name = OptionalValue<string>.Unspecified,
                Age = OptionalValue<int>.Unspecified
            },
            Street = OptionalValue<string>.Unspecified,
            HouseNumber = OptionalValue<int>.Unspecified
        };

        var options = CreateOptionsMultipleContexts();
        var json = JsonSerializer.Serialize(model, options);

        Assert.Equal("""{"Test":{}}""", json);
    }

    [Fact]
    public void DeserializeWithValuesMultipleContexts_ShouldReadValues()
    {
        var json = """{"Test":{"Name":"John","Age":42},"Street":"Main Street","HouseNumber":42}""";

        var options = CreateOptionsMultipleContexts();
        var model = JsonSerializer.Deserialize<TestModelOtherContext>(json, options)!;

        Assert.Equal("John", model.Test.SpecifiedValue.Name.Value);
        Assert.Equal(42, model.Test.SpecifiedValue.Age.Value);
        Assert.Equal("Main Street", model.Street.Value);
        Assert.Equal(42, model.HouseNumber.Value);
    }

    [Fact]
    public void DeserializeWithUnspecifiedMultipleContexts_ShouldReadUnspecified()
    {
        var json = """{"Test":{}}""";

        var options = CreateOptionsMultipleContexts();
        var model = JsonSerializer.Deserialize<TestModelOtherContext>(json, options)!;

        Assert.True(model.Test.IsSpecified);

        Assert.False(model.Test.SpecifiedValue.Name.IsSpecified);
        Assert.False(model.Test.SpecifiedValue.Age.IsSpecified);
        Assert.False(model.Street.IsSpecified);
        Assert.False(model.HouseNumber.IsSpecified);
    }

    public class TestModel
    {
        public OptionalValue<string> Name { get; set; }

        public OptionalValue<int> Age { get; set; }
    }

    public class TestModelOtherContext
    {
        public OptionalValue<TestModel> Test { get; set; }

        public OptionalValue<string> Street { get; set; }

        public OptionalValue<int> HouseNumber { get; set; }
    }
}

[JsonSerializable(typeof(OptionalValueJsonWithSourceGeneratorTest.TestModel))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
public partial class OptionalValueJsonWithSourceGeneratorJsonSerializationContext : JsonSerializerContext
{
}

[JsonSerializable(typeof(OptionalValueJsonWithSourceGeneratorTest.TestModelOtherContext))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
public partial class OtherOptionalValueJsonWithSourceGeneratorJsonSerializationContext : JsonSerializerContext
{
}