using System.ComponentModel.DataAnnotations;

using Shouldly;

namespace OptionalValues.DataAnnotations.Tests;

/// <summary>
/// Tests that verify OptionalValues DataAnnotations work with .NET 10's minimal API validation support.
/// In .NET 10, minimal APIs automatically validate models using ValidationContext with service provider support.
/// See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0#validation-support-in-minimal-apis
/// 
/// These tests verify that OptionalValues DataAnnotations work correctly when ValidationContext includes
/// an IServiceProvider, which enables nested object validation as used by minimal APIs.
/// </summary>
public class MinimalApiValidationTest
{
    public class TestModel
    {
        [RequiredValue]
        public OptionalValue<string> RequiredName { get; init; }

        [OptionalRange(1, 100)]
        public OptionalValue<int> RangeValue { get; init; }

        [Specified]
        public OptionalValue<string?> SpecifiedField { get; init; }
    }

    [Fact]
    public void ValidModel_ShouldPass_WithServiceProvider()
    {
        // Arrange
        var validModel = new TestModel
        {
            RequiredName = "TestName",
            RangeValue = 50,
            SpecifiedField = "value"
        };

        // Act - ValidationContext with IServiceProvider enables nested validation
        var context = new ValidationContext(validModel, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(validModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeTrue();
        results.ShouldBeEmpty();
    }

    [Fact]
    public void InvalidModel_ShouldFail_WithServiceProvider()
    {
        // Arrange
        var invalidModel = new TestModel
        {
            RequiredName = OptionalValue<string>.Unspecified,
            RangeValue = 150,
            SpecifiedField = OptionalValue<string?>.Unspecified
        };

        // Act - ValidationContext with IServiceProvider enables nested validation
        var context = new ValidationContext(invalidModel, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(invalidModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeFalse();
        results.ShouldNotBeEmpty();
    }

    [Fact]
    public void UnspecifiedOptionalFields_ShouldBeValid()
    {
        // Arrange
        var validModel = new TestModel
        {
            RequiredName = "Test",
            SpecifiedField = null // Specified allows null, just not unspecified
            // RangeValue is unspecified, which should be valid for optional fields
        };

        // Act - ValidationContext with IServiceProvider enables nested validation
        var context = new ValidationContext(validModel, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(validModel, context, results, validateAllProperties: true);

        // Assert
        isValid.ShouldBeTrue();
        results.ShouldBeEmpty();
    }
}
