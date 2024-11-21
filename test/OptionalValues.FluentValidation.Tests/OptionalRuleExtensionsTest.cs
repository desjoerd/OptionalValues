using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;

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

        Assert.True(result.IsValid);
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

        Assert.False(result.IsValid);

        result.Errors.Select(x => x.PropertyName)
            .Should()
            .BeEquivalentTo(nameof(TestData.FirstName), nameof(TestData.Age));
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

        Assert.True(result.IsValid);
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
