using System.Text.Json;
using System.Text.Json.Serialization;

using Shouldly;

namespace OptionalValues.Tests;

public class OptionalValueJsonTest
{
    public class WithoutJsonTypeResolver : OptionalValueTest
    {
        public class Serialize : WithoutJsonTypeResolver
        {
            [Fact]
            public void Should_Omit_When_Unspecified()
            {
                var testData = new TestDataSingle<string>(default);

                var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

                json.ShouldBe("{}");
            }

            [Fact]
            public void Should_Write_Null_When_Specified_Null()
            {
                var testData = new TestDataSingle<string?>(null);

                var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

                json.ShouldBe("""{"SingleValue":null}""");
            }

            [Fact]
            public void Should_Write_Value_When_Specified_Value()
            {
                var testData = new TestDataSingle<string>("foo");

                var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

                json.ShouldBe("""{"SingleValue":"foo"}""");
            }

            [Fact]
            public void Should_Serialize_String_Omitting_Unspecified()
            {
                var testData = new TestDataMultiple<string>("foo");

                var json = JsonSerializer.Serialize(testData, JsonSerializerOptions.Default);

                json.ShouldBe("""{"ProvidedValue":"foo","ValueNullOrDefault":null}""");
            }

            [Fact]
            public void Should_Serialize_Int_Omitting_Unspecified()
            {
                var testData = new TestDataMultiple<int>(30);

                var json = JsonSerializer.Serialize(testData);

                json.ShouldBe("""{"ProvidedValue":30,"ValueNullOrDefault":0}""");
            }

            [Fact]
            public void Should_Serialize_NullableInt_Omitting_Unspecified()
            {
                var testData = new TestDataMultiple<int?>(30);

                var json = JsonSerializer.Serialize(testData);

                json.ShouldBe("""{"ProvidedValue":30,"ValueNullOrDefault":null}""");
            }

            [Fact]
            public void Should_Throw_When_Property_Is_Not_Annotated_With_Ignore_When_Writing_Default()
            {
                var invalidTestData = new InvalidTestData(OptionalValue<string>.Unspecified);

                Action action = () => JsonSerializer.Serialize(invalidTestData);
                action.ShouldThrow<InvalidOperationException>()
                    .Message.ShouldBe("Value is unspecified. Writing the property would give a property without any value, resulting in invalid json. " +
                                      "Add OptionalValue support via 'AddOptionalValueSupport()' on the 'JsonSerializerOptions' or " +
                                      "set [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] on the property.");
            }
        }

        public class Deserialize : WithoutJsonTypeResolver
        {
            [Fact]
            public void Should_Deserialize_String()
            {
                const string json = """{"ProvidedValue":"foo","ValueNullOrDefault":null}""";

                var testData = JsonSerializer.Deserialize<TestDataMultiple<string>>(json)!;

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe("foo");

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Deserialize_Int()
            {
                const string json = """{"ProvidedValue":30,"ValueNullOrDefault":0}""";

                var testData = JsonSerializer.Deserialize<TestDataMultiple<int>>(json)!;

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe(30);

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBe(0);

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Deserialize_NullableInt()
            {
                const string json = """{"ProvidedValue":30,"ValueNullOrDefault":null}""";

                var testData = JsonSerializer.Deserialize<TestDataMultiple<int?>>(json)!;

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe(30);

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Deserialize_Nested()
            {
                const string json = """{"SingleValue":{"SingleValue": null}}""";

                var testData = JsonSerializer.Deserialize<TestDataSingle<TestDataSingle<string?>>>(json)!;

                testData.SingleValue.IsSpecified.ShouldBeTrue();
                testData.SingleValue.SpecifiedValue.ShouldBe(new TestDataSingle<string?>(null));
            }
        }

        public class SerializeDeserialize : OptionalValueJsonTest
        {
            [Fact]
            public void Should_Serialize_And_Deserialize_String()
            {
                var testData = new TestDataMultiple<string>("foo");

                var json = JsonSerializer.Serialize(testData);
                var deserialized = JsonSerializer.Deserialize<TestDataMultiple<string>>(json)!;

                deserialized.ProvidedValue.IsSpecified.ShouldBeTrue();
                deserialized.ProvidedValue.SpecifiedValue.ShouldBe("foo");

                deserialized.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                deserialized.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                deserialized.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Serialize_And_Deserialize_Int()
            {
                var testData = new TestDataMultiple<int>(30);

                var json = JsonSerializer.Serialize(testData);
                var deserialized = JsonSerializer.Deserialize<TestDataMultiple<int>>(json)!;

                deserialized.ProvidedValue.IsSpecified.ShouldBeTrue();
                deserialized.ProvidedValue.SpecifiedValue.ShouldBe(30);

                deserialized.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                deserialized.ValueNullOrDefault.SpecifiedValue.ShouldBe(0);

                deserialized.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Serialize_And_Deserialize_NullableInt()
            {
                var testData = new TestDataMultiple<int?>(30);

                var json = JsonSerializer.Serialize(testData);
                var deserialized = JsonSerializer.Deserialize<TestDataMultiple<int?>>(json)!;

                deserialized.ProvidedValue.IsSpecified.ShouldBeTrue();
                deserialized.ProvidedValue.SpecifiedValue.ShouldBe(30);

                deserialized.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                deserialized.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                deserialized.Unspecified.IsSpecified.ShouldBeFalse();
            }
        }

        public class WithoutTypeResolverTestDataMultipleExpectations : OptionalValueJsonTest
        {
            [Fact]
            public void Verify_TestData_String()
            {
                var testData = new TestDataMultiple<string>("foo");

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe("foo");

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Verify_TestData_Int()
            {
                var testData = new TestDataMultiple<int>(30);

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe(30);

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBe(0);

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Verify_TestData_NullableInt()
            {
                var testData = new TestDataMultiple<int?>(30);

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe(30);

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }
        }

        private record TestDataSingle<T>(
            [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            OptionalValue<T> SingleValue);

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
            public OptionalValue<T> ValueNullOrDefault { get; init; } = default(T)!;

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public OptionalValue<T> Unspecified { get; init; }
        }

        private record InvalidTestData(OptionalValue<string> Value);
    }

    public class WithJsonTypeResolver : OptionalValueTest
    {
        protected static JsonSerializerOptions SerializerOptionsWithTypeResolver = JsonSerializerOptions.Default
            .WithOptionalValueSupport();

        public class Serialize : WithJsonTypeResolver
        {
            [Fact]
            public void Should_Omit_When_Unspecified()
            {
                var testData = new TestDataSingle<string>(default);

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);

                json.ShouldBe("{}");
            }

            [Fact]
            public void Should_Write_Null_When_Specified_Null()
            {
                var testData = new TestDataSingle<string?>(null);

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);

                json.ShouldBe("""{"SingleValue":null}""");
            }

            [Fact]
            public void Should_Write_Value_When_Specified_Value()
            {
                var testData = new TestDataSingle<string>("foo");

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);

                json.ShouldBe("""{"SingleValue":"foo"}""");
            }

            [Fact]
            public void Should_Serialize_String_Omitting_Unspecified()
            {
                var testData = new TestDataMultiple<string>("foo");

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);

                json.ShouldBe("""{"ProvidedValue":"foo","ValueNullOrDefault":null}""");
            }

            [Fact]
            public void Should_Serialize_Int_Omitting_Unspecified()
            {
                var testData = new TestDataMultiple<int>(30);

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);

                json.ShouldBe("""{"ProvidedValue":30,"ValueNullOrDefault":0}""");
            }

            [Fact]
            public void Should_Serialize_NullableInt_Omitting_Unspecified()
            {
                var testData = new TestDataMultiple<int?>(30);

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);

                json.ShouldBe("""{"ProvidedValue":30,"ValueNullOrDefault":null}""");
            }
        }

        public class Deserialize : WithJsonTypeResolver
        {
            [Fact]
            public void Should_Deserialize_String()
            {
                const string json = """{"ProvidedValue":"foo","ValueNullOrDefault":null}""";

                var testData = JsonSerializer.Deserialize<TestDataMultiple<string?>>(json, SerializerOptionsWithTypeResolver)!;

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe("foo");

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Deserialize_Int()
            {
                const string json = """{"ProvidedValue":30,"ValueNullOrDefault":0}""";

                var testData = JsonSerializer.Deserialize<TestDataMultiple<int>>(json, SerializerOptionsWithTypeResolver)!;

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe(30);

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBe(0);

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Deserialize_NullableInt()
            {
                const string json = """{"ProvidedValue":30,"ValueNullOrDefault":null}""";

                var testData = JsonSerializer.Deserialize<TestDataMultiple<int?>>(json, SerializerOptionsWithTypeResolver)!;

                testData.ProvidedValue.IsSpecified.ShouldBeTrue();
                testData.ProvidedValue.SpecifiedValue.ShouldBe(30);

                testData.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                testData.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                testData.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Deserialize_Nested()
            {
                const string json = """{"SingleValue":{"SingleValue": null}}""";

                var testData = JsonSerializer.Deserialize<TestDataSingle<TestDataSingle<string?>>>(json, SerializerOptionsWithTypeResolver)!;

                testData.SingleValue.IsSpecified.ShouldBeTrue();
                testData.SingleValue.SpecifiedValue.ShouldBe(new TestDataSingle<string?>(null));
            }
        }

        public class SerializeDeserialize : WithJsonTypeResolver
        {
            [Fact]
            public void Should_Serialize_And_Deserialize_String()
            {
                var testData = new TestDataMultiple<string>("foo");

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);
                var deserialized = JsonSerializer.Deserialize<TestDataMultiple<string>>(json, SerializerOptionsWithTypeResolver)!;

                deserialized.ProvidedValue.IsSpecified.ShouldBeTrue();
                deserialized.ProvidedValue.SpecifiedValue.ShouldBe("foo");

                deserialized.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                deserialized.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                deserialized.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Serialize_And_Deserialize_Int()
            {
                var testData = new TestDataMultiple<int>(30);

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);
                var deserialized = JsonSerializer.Deserialize<TestDataMultiple<int>>(json, SerializerOptionsWithTypeResolver)!;

                deserialized.ProvidedValue.IsSpecified.ShouldBeTrue();
                deserialized.ProvidedValue.SpecifiedValue.ShouldBe(30);

                deserialized.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                deserialized.ValueNullOrDefault.SpecifiedValue.ShouldBe(0);

                deserialized.Unspecified.IsSpecified.ShouldBeFalse();
            }

            [Fact]
            public void Should_Serialize_And_Deserialize_NullableInt()
            {
                var testData = new TestDataMultiple<int?>(30);

                var json = JsonSerializer.Serialize(testData, SerializerOptionsWithTypeResolver);
                var deserialized = JsonSerializer.Deserialize<TestDataMultiple<int?>>(json, SerializerOptionsWithTypeResolver)!;

                deserialized.ProvidedValue.IsSpecified.ShouldBeTrue();
                deserialized.ProvidedValue.SpecifiedValue.ShouldBe(30);

                deserialized.ValueNullOrDefault.IsSpecified.ShouldBeTrue();
                deserialized.ValueNullOrDefault.SpecifiedValue.ShouldBeNull();

                deserialized.Unspecified.IsSpecified.ShouldBeFalse();
            }
        }

        private record TestDataSingle<T>(OptionalValue<T> SingleValue);

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

            public OptionalValue<T> ProvidedValue { get; init; }

            public OptionalValue<T> ValueNullOrDefault { get; init; } = default(T)!;

            public OptionalValue<T> Unspecified { get; init; }
        }
    }
}
