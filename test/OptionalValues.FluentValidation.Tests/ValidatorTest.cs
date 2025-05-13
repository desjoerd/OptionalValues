using FluentValidation;
using FluentValidation.Results;

using Xunit.Abstractions;

namespace OptionalValues.FluentValidation.Tests;

public class ValidatorTests(ITestOutputHelper testOutputHelper)
{
    public class TestData
    {
        public OptionalValue<string> FirstName { get; set; } = default!;
        public OptionalValue<int> Age { get; set; } = default!;
    }

    public class TestDataValidator : AbstractValidator<TestData>
    {
        public TestDataValidator()
        {
            this.RuleForOptionalValue(x => x.FirstName)
                .MinimumLength(3)
                .WithMessage("First name must be at least 3 characters long.");

            this.RuleForOptionalValue(x => x.Age)
                .GreaterThan(18)
                .WithMessage("Age must be greater than 18.");
        }
    }

    [Fact]
    public void ValidUnspecified()
    {
        var validTestData = new TestData
        {
            FirstName = default,
            Age = default,
        };

        var validator = new TestDataValidator();
        ValidationResult? result = validator.Validate(validTestData);

        foreach(var error in result.Errors)
        {
            testOutputHelper.WriteLine($"Error: {error.ErrorMessage}");
        }

        Assert.True(result.IsValid);
    }
}