using System.ComponentModel.DataAnnotations;

using Shouldly;

namespace OptionalValues.DataAnnotations.Tests;

/// <summary>
/// Tests that verify OptionalValues DataAnnotations work with .NET 10's minimal API validation support.
/// In .NET 10, minimal APIs automatically validate models decorated with DataAnnotation attributes.
/// See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0#validation-support-in-minimal-apis
/// 
/// These tests verify that OptionalValues DataAnnotations work correctly with the standard
/// validation framework, which is used by minimal APIs in .NET 10.
/// </summary>
public class MinimalApiValidationTest
{
    public class TestModel
    {
        [RequiredValue]
        public OptionalValue<string> RequiredName { get; init; }

        [OptionalLength(1, 10)]
        public OptionalValue<string> LimitedText { get; init; }

        [OptionalRange(1, 100)]
        public OptionalValue<int> RangeValue { get; init; }

        [Specified]
        public OptionalValue<string?> SpecifiedField { get; init; }

        [OptionalRegularExpression("^[A-Z]+$")]
        public OptionalValue<string> UppercaseOnly { get; init; }

        [OptionalMinLength(2)]
        public OptionalValue<string> MinLengthText { get; init; }

        [OptionalMaxLength(5)]
        public OptionalValue<string> MaxLengthText { get; init; }

        [OptionalBase64String]
        public OptionalValue<string> Base64String { get; init; }

        [OptionalAllowedValues("value1", "value2")]
        public OptionalValue<string> AllowedValues { get; init; }

        [OptionalDeniedValues("forbidden")]
        public OptionalValue<string> DeniedValues { get; init; }

        [OptionalStringLength(20)]
        public OptionalValue<string> StringLength { get; init; }
    }

    [Fact]
    public void ValidModel_ShouldPass_MinimalApiValidation()
    {
        // Arrange
        var validModel = new TestModel
        {
            RequiredName = "TestName",
            LimitedText = "Valid",
            RangeValue = 50,
            SpecifiedField = "value",
            UppercaseOnly = "UPPERCASE",
            MinLengthText = "ab",
            MaxLengthText = "short",
            Base64String = "dmFsaWQ=", // "valid" in base64
            AllowedValues = "value1",
            DeniedValues = "allowed",
            StringLength = "test"
        };

        // Act
        var context = new ValidationContext(validModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(validModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeTrue();
        results.ShouldBeEmpty();
    }

    [Fact]
    public void RequiredValue_WhenUnspecified_ShouldFail_MinimalApiValidation()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = OptionalValue<string>.Unspecified,
            SpecifiedField = "value"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.RequiredName)));
    }

    [Fact]
    public void OptionalLength_WhenTooLong_ShouldFail_MinimalApiValidation()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            LimitedText = "ThisIsTooLong",
            SpecifiedField = "value"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.LimitedText)));
    }

    [Fact]
    public void OptionalRange_WhenOutOfRange_ShouldFail_MinimalApiValidation()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            RangeValue = 150,
            SpecifiedField = "value"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.RangeValue)));
    }

    [Fact]
    public void Specified_WhenUnspecified_ShouldFail_MinimalApiValidation()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = OptionalValue<string?>.Unspecified
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.SpecifiedField)));
    }

    [Fact]
    public void OptionalRegularExpression_WhenInvalid_ShouldFail_MinimalApiValidation()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = "value",
            UppercaseOnly = "lowercase"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.UppercaseOnly)));
    }

    [Fact]
    public void OptionalMinLength_WhenTooShort_ShouldFail_MinimalApiValidation()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = "value",
            MinLengthText = "a"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.MinLengthText)));
    }

    [Fact]
    public void OptionalMaxLength_WhenTooLong_ShouldFail_MinimalApiValidation()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = "value",
            MaxLengthText = "toolongtext"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.MaxLengthText)));
    }

    [Fact]
    public void UnspecifiedOptionalFields_ShouldBeValid_MinimalApiValidation()
    {
        // Arrange - only required fields are specified
        var validModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = null // Specified allows null, just not unspecified
        };

        // Act
        var context = new ValidationContext(validModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(validModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeTrue();
        results.ShouldBeEmpty();
    }

#if NET10_0_OR_GREATER
    [Fact]
    public void OptionalBase64String_WhenInvalid_ShouldFail_Net10()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = "value",
            Base64String = "invalid!"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.Base64String)));
    }

    [Fact]
    public void OptionalAllowedValues_WhenNotAllowed_ShouldFail_Net10()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = "value",
            AllowedValues = "notallowed"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.AllowedValues)));
    }

    [Fact]
    public void OptionalDeniedValues_WhenDenied_ShouldFail_Net10()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = "value",
            DeniedValues = "forbidden"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.DeniedValues)));
    }

    [Fact]
    public void OptionalStringLength_WhenTooLong_ShouldFail_Net10()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = "value",
            StringLength = "ThisIsWayTooLongForTheLimit"
        };

        // Act
        var context = new ValidationContext(invalidModel);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldContain(r => r.MemberNames.Contains(nameof(TestModel.StringLength)));
    }
#endif
}
