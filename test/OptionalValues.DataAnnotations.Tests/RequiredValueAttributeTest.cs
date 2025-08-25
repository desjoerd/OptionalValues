using System.ComponentModel.DataAnnotations;

using Shouldly;

namespace OptionalValues.DataAnnotations.Tests;

public class RequiredValueAttributeTest
{
    private class Example
    {
        [RequiredValue]
        public OptionalValue<bool?> RequiredOptionalBool { get; set; }
    }

    [Fact]
    public void Should_Mark_Unspecified_As_Required()
    {
        var model = new Example
        {
            RequiredOptionalBool = OptionalValue<bool?>.Unspecified,
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeFalse();
        results.Count.ShouldBe(1);
        results[0].MemberNames.ShouldBeEquivalentTo(new[] { nameof(Example.RequiredOptionalBool) });
    }

    [Fact]
    public void Should_Mark_Null_As_Required()
    {
        var model = new Example
        {
            RequiredOptionalBool = null,
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeFalse();
        results.Count.ShouldBe(1);
        results[0].MemberNames.ShouldBeEquivalentTo(new[] { nameof(Example.RequiredOptionalBool) });
    }
}
