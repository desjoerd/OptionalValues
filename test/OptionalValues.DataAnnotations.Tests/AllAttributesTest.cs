using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Shouldly;

namespace OptionalValues.DataAnnotations.Tests;

public class AllAttributesTest
{
    public class TestModel
    {
        [OptionalAllowedValues("a")]
        public OptionalValue<string> AllowedValues { get; set; }

        [OptionalBase64String]
        public OptionalValue<string> Base64String { get; set; }

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

        [OptionalRequired(AllowUnspecified = true)]
        public OptionalValue<string> Required { get; set; }

        [OptionalStringLength(5)]
        public OptionalValue<string> StringLength { get; set; }

        public static TestModel CreateUnspecified() => new();

        public static TestModel CreateValid()
            => new()
            {
                AllowedValues = "a",
                Base64String = "dmFsaWQ=", // "valid" in base64
                DeniedValues = "b",
                LengthCollection = new[] { 1, 2, 3, 4, 5 },
                LengthString = "12345",
                MaxLengthCollection = new[] { 1, 2, 3, 4, 5 },
                MaxLengthString = "12345",
                MinLengthCollection = new[] { 1 },
                MinLengthString = "1",
                Range = 42,
                RegularExpression = "something",
                Required = "something",
                StringLength = "12345",
            };

        public static TestModel CreateInvalid()
            => new()
            {
                AllowedValues = "b",
                Base64String = "invalid!",
                DeniedValues = "a",
                LengthCollection = new[] { 1, 2, 3, 4, 5, 6 },
                LengthString = "123456",
                MaxLengthCollection = new[] { 1, 2, 3, 4, 5, 6 },
                MaxLengthString = "123456",
                MinLengthCollection = Array.Empty<int>(),
                MinLengthString = "",
                Range = 4,
                RegularExpression = "something else",
                Required = "",
                StringLength = "123456",
            };
    }

    [Fact]
    public void ValidValuesShouldBeValid()
    {
        var model = TestModel.CreateValid();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeTrue();
        results.ShouldBeEmpty();
    }

    [Fact]
    public void UnspecifiedValuesShouldBeValid()
    {
        var model = TestModel.CreateUnspecified();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeTrue();
        results.ShouldBeEmpty();
    }

    [Fact]
    public void InvalidValuesShouldBeInvalid()
    {
        var model = TestModel.CreateInvalid();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeFalse();
        results.Count.ShouldBe(typeof(TestModel).GetProperties().Length);

        foreach (PropertyInfo property in typeof(TestModel).GetProperties())
        {
            results.ShouldContain(r => r.MemberNames.Contains(property.Name));
        }
    }
}
