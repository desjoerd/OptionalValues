using FluentValidation;
using FluentValidation.Results;

using Shouldly;

namespace OptionalValues.FluentValidation.Tests;

public class OptionalRuleExtensionsTest
{
    [Fact]
    public void Valid()
    {
        var validTestData = new TestData
        {
            FirstName = "John",
            Age = 30,
        };

        var validator = new TestDataValidator();

        ValidationResult? result = validator.Validate(validTestData);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid()
    {
        var invalidTestData = new TestData
        {
            FirstName = "Jo",
            Age = 17,
        };

        var validator = new TestDataValidator();

        ValidationResult? result = validator.Validate(invalidTestData);

        result.IsValid.ShouldBeFalse();

        result.Errors.Select(x => x.PropertyName)
            .ShouldBe([nameof(TestData.FirstName), nameof(TestData.Age)]);
    }

    [Fact]
    public void SkipUndefinedValues()
    {
        var partialValidTestData = new TestData
        {
            FirstName = "John",
            Age = default,
        };

        var validator = new TestDataValidator();

        ValidationResult? result = validator.Validate(partialValidTestData);

        result.IsValid.ShouldBeTrue();
    }

    public class TestData
    {
        public OptionalValue<string?> FirstName { get; set; }
        public OptionalValue<int> Age { get; set; }
    }

    public class TestDataValidator : AbstractValidator<TestData>
    {
        public TestDataValidator()
        {
            this.OptionalRuleFor(x => x.FirstName, x => x
                .NotEmpty()
                .MinimumLength(3));

            this.OptionalRuleFor(x => x.Age, x => x
                .GreaterThan(18));
        }
    }
}
