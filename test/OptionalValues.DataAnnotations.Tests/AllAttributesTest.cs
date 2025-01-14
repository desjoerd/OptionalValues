using System.ComponentModel.DataAnnotations;
using System.Reflection;

using FluentAssertions;

namespace OptionalValues.DataAnnotations.Tests;

public class AllAttributesTest
{
    public class TestModel
    {
        [OptionalAllowedValues("a")]
        public OptionalValue<string> AllowedValues { get; set; }

        [OptionalDeniedValues("a")]
        public OptionalValue<string> DeniedValues { get; set; }

        [OptionalLength(1, 5)]
        public OptionalValue<int[]> LengthCollection { get; set; }

        [OptionalLength(1, 5)]
        public OptionalValue<string> LengthString { get; set; }

        [OptionalMaxLength(5)]
        public OptionalValue<int[]> MaxLengthCollection { get; set; }

        [OptionalMaxLength(5)]
        public OptionalValue<string> MaxLengthString { get; set; }

        [OptionalMinLength(1)]
        public OptionalValue<int[]> MinLengthCollection { get; set; }

        [OptionalMinLength(1)]
        public OptionalValue<string> MinLengthString { get; set; }

        [OptionalRange(5, 42)]
        public OptionalValue<int> Range { get; set; }

        [OptionalRegularExpression("^something$")]
        public OptionalValue<string> RegularExpression { get; set; }

        [OptionalStringLength(5)]
        public OptionalValue<string> StringLength { get; set; }

        public static TestModel CreateUnspecified() => new();

        public static TestModel CreateValid()
            => new()
            {
                AllowedValues = "a",
                DeniedValues = "b",
                LengthCollection = new[] { 1, 2, 3, 4, 5 },
                LengthString = "12345",
                MaxLengthCollection = new[] { 1, 2, 3, 4, 5 },
                MaxLengthString = "12345",
                MinLengthCollection = new[] { 1 },
                MinLengthString = "1",
                Range = 42,
                RegularExpression = "something",
                StringLength = "12345",
            };

        public static TestModel CreateInvalid()
            => new()
            {
                AllowedValues = "b",
                DeniedValues = "a",
                LengthCollection = new[] { 1, 2, 3, 4, 5, 6 },
                LengthString = "123456",
                MaxLengthCollection = new[] { 1, 2, 3, 4, 5, 6 },
                MaxLengthString = "123456",
                MinLengthCollection = Array.Empty<int>(),
                MinLengthString = "",
                Range = 4,
                RegularExpression = "something else",
                StringLength = "123456",
            };
    }

    [Fact]
    public void ValidValuesShouldBeValid()
    {
        var model = TestModel.CreateValid();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void UnspecifiedValuesShouldBeValid()
    {
        var model = TestModel.CreateUnspecified();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void InvalidValuesShouldBeInvalid()
    {
        var model = TestModel.CreateInvalid();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).Should().BeFalse();
        results.Should().HaveCount(typeof(TestModel).GetProperties().Length);

        foreach(PropertyInfo property in typeof(TestModel).GetProperties())
        {
            results.Should().ContainSingle(r => r.MemberNames.Contains(property.Name));
        }
    }
}