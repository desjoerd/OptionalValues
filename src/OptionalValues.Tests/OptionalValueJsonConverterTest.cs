using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;

namespace OptionalValues.Tests;

public class OptionalValueJsonConverterTest
{
    public class Serialize : OptionalValueJsonConverterTest
    {
        [Fact]
        public void Should_Omit_When_Unspecified()
        {
            var testData = new TestDataSingle<string>(default);

            var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

            json.Should().Be("{}");
        }

        [Fact]
        public void Should_Write_Null_When_Specified_Null()
        {
            var testData = new TestDataSingle<string?>(null);

            var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

            json.Should().Be("""{"SingleValue":null}""");
        }

        [Fact]
        public void Should_Write_Value_When_Specified_Value()
        {
            var testData = new TestDataSingle<string>("foo");

            var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

            json.Should().Be("""{"SingleValue":"foo"}""");
        }

        [Fact]
        public void Should_Serialize_String_Omitting_Unspecified()
        {
            var testData = new TestDataMultiple<string>("foo");

            var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

            json.Should().Be("""{"ProvidedValue":"foo","ValueDefault":null}""");
        }

        [Fact]
        public void Should_Serialize_Int_Omitting_Unspecified()
        {
            var testData = new TestDataMultiple<int>(30);

            var json = JsonSerializer.Serialize(testData);

            json.Should().Be("""{"ProvidedValue":30,"ValueDefault":0}""");
        }

        [Fact]
        public void Should_Serialize_NullableInt_Omitting_Unspecified()
        {
            var testData = new TestDataMultiple<int?>(30);

            var json = JsonSerializer.Serialize(testData);

            json.Should().Be("""{"ProvidedValue":30,"ValueDefault":null}""");
        }

        [Fact]
        public void Should_Throw_When_Property_Is_Not_Annotated_With_Ignore_When_Writing_Default()
        {
            var invalidTestData = new InvalidTestData(OptionalValue<string>.Unspecified);

            Action action = () => JsonSerializer.Serialize(invalidTestData);
            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Value is unspecified. Please set [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] on the property.");
        }
    }

    public class Deserialize : OptionalValueJsonConverterTest
    {
        [Fact]
        public void Should_Deserialize_String()
        {
            const string json = """{"ProvidedValue":"foo","ValueDefault":null}""";

            var testData = JsonSerializer.Deserialize<TestDataMultiple<string>>(json)!;

            Assert.True(testData.ProvidedValue.IsSpecified);
            testData.ProvidedValue.SpecifiedValue.Should().Be("foo");

            Assert.True(testData.ValueDefault.IsSpecified);
            testData.ValueDefault.SpecifiedValue.Should().Be(null);

            Assert.False(testData.Unspecified.IsSpecified);
        }

        [Fact]
        public void Should_Deserialize_Int()
        {
            const string json = """{"ProvidedValue":30,"ValueDefault":0}""";

            var testData = JsonSerializer.Deserialize<TestDataMultiple<int>>(json)!;

            Assert.True(testData.ProvidedValue.IsSpecified);
            testData.ProvidedValue.SpecifiedValue.Should().Be(30);

            Assert.True(testData.ValueDefault.IsSpecified);
            testData.ValueDefault.SpecifiedValue.Should().Be(0);

            Assert.False(testData.Unspecified.IsSpecified);
        }

        [Fact]
        public void Should_Deserialize_NullableInt()
        {
            const string json = """{"ProvidedValue":30,"ValueDefault":null}""";

            var testData = JsonSerializer.Deserialize<TestDataMultiple<int?>>(json)!;

            Assert.True(testData.ProvidedValue.IsSpecified);
            testData.ProvidedValue.SpecifiedValue.Should().Be(30);

            Assert.True(testData.ValueDefault.IsSpecified);
            testData.ValueDefault.SpecifiedValue.Should().Be(null);

            Assert.False(testData.Unspecified.IsSpecified);
        }

        [Fact]
        public void Should_Deserialize_Nested()
        {
            const string json = """{"SingleValue":{"SingleValue": null}}""";

            var testData = JsonSerializer.Deserialize<TestDataSingle<TestDataSingle<string?>>>(json)!;

            Assert.True(testData.SingleValue.IsSpecified);
            testData.SingleValue.SpecifiedValue.Should().Be(new TestDataSingle<string?>(null));
        }
    }

    public class SerializeDeserialize : OptionalValueJsonConverterTest
    {
        [Fact]
        public void Should_Serialize_And_Deserialize_String()
        {
            var testData = new TestDataMultiple<string>("foo");

            var json = JsonSerializer.Serialize(testData);
            var deserialized = JsonSerializer.Deserialize<TestDataMultiple<string>>(json)!;

            Assert.True(deserialized.ProvidedValue.IsSpecified);
            deserialized.ProvidedValue.SpecifiedValue.Should().Be("foo");

            Assert.True(deserialized.ValueDefault.IsSpecified);
            deserialized.ValueDefault.SpecifiedValue.Should().Be(null);

            Assert.False(deserialized.Unspecified.IsSpecified);
        }

        [Fact]
        public void Should_Serialize_And_Deserialize_Int()
        {
            var testData = new TestDataMultiple<int>(30);

            var json = JsonSerializer.Serialize(testData);
            var deserialized = JsonSerializer.Deserialize<TestDataMultiple<int>>(json)!;

            Assert.True(deserialized.ProvidedValue.IsSpecified);
            deserialized.ProvidedValue.SpecifiedValue.Should().Be(30);

            Assert.True(deserialized.ValueDefault.IsSpecified);
            deserialized.ValueDefault.SpecifiedValue.Should().Be(0);

            Assert.False(deserialized.Unspecified.IsSpecified);
        }

        [Fact]
        public void Should_Serialize_And_Deserialize_NullableInt()
        {
            var testData = new TestDataMultiple<int?>(30);

            var json = JsonSerializer.Serialize(testData);
            var deserialized = JsonSerializer.Deserialize<TestDataMultiple<int?>>(json)!;

            Assert.True(deserialized.ProvidedValue.IsSpecified);
            deserialized.ProvidedValue.SpecifiedValue.Should().Be(30);

            Assert.True(deserialized.ValueDefault.IsSpecified);
            deserialized.ValueDefault.SpecifiedValue.Should().Be(null);

            Assert.False(deserialized.Unspecified.IsSpecified);
        }
    }

    public class TestDataMultipleExpectations : OptionalValueJsonConverterTest
    {
        [Fact]
        public void Verify_TestData_String()
        {
            var testData = new TestDataMultiple<string>("foo");

            Assert.True(testData.ProvidedValue.IsSpecified);
            testData.ProvidedValue.SpecifiedValue.Should().Be("foo");

            Assert.True(testData.ValueDefault.IsSpecified);
            testData.ValueDefault.SpecifiedValue.Should().Be(null);

            Assert.False(testData.Unspecified.IsSpecified);
        }

        [Fact]
        public void Verify_TestData_Int()
        {
            var testData = new TestDataMultiple<int>(30);

            Assert.True(testData.ProvidedValue.IsSpecified);
            testData.ProvidedValue.SpecifiedValue.Should().Be(30);

            Assert.True(testData.ValueDefault.IsSpecified);
            testData.ValueDefault.SpecifiedValue.Should().Be(0);

            Assert.False(testData.Unspecified.IsSpecified);
        }

        [Fact]
        public void Verify_TestData_NullableInt()
        {
            var testData = new TestDataMultiple<int?>(30);

            Assert.True(testData.ProvidedValue.IsSpecified);
            testData.ProvidedValue.SpecifiedValue.Should().Be(30);

            Assert.True(testData.ValueDefault.IsSpecified);
            testData.ValueDefault.SpecifiedValue.Should().Be(null);

            Assert.False(testData.Unspecified.IsSpecified);
        }
    }

    private record TestDataSingle<T>([property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] OptionalValue<T> SingleValue);

    private record TestDataMultiple<T>
    {
        public TestDataMultiple(T providedValue)
        {
            ProvidedValue = providedValue;
        }

        [JsonConstructor]
        public TestDataMultiple()
        {
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OptionalValue<T> ProvidedValue { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OptionalValue<T> ValueDefault { get; init; } = default(T)!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OptionalValue<T> Unspecified { get; init; }
    }

    private record InvalidTestData(OptionalValue<string> Value);
}