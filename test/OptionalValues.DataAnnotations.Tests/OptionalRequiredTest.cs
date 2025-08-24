using System.ComponentModel.DataAnnotations;

using Shouldly;

namespace OptionalValues.DataAnnotations.Tests;

public class OptionalRequiredTest
{
    public class Example
    {
        [OptionalRequired]
        public OptionalValue<bool?> RequiredOptionalBool { get; set; }

        [OptionalRequired(AllowUnspecified = true)]
        public OptionalValue<bool?> RequiredOptionalBoolAllowsUnspecified { get; set; }
    }

    [Fact]
    public void Should_Mark_Unspecified_As_Required()
    {
        var model = new Example
        {
            RequiredOptionalBool = OptionalValue<bool?>.Unspecified,

            // This is fine, as it allows unspecified
            RequiredOptionalBoolAllowsUnspecified = OptionalValue<bool?>.Unspecified,
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
            RequiredOptionalBoolAllowsUnspecified = null,
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeFalse();
        results.Count.ShouldBe(2);
        results[0].MemberNames.ShouldBeEquivalentTo(new[] { nameof(Example.RequiredOptionalBool) });
        results[1].MemberNames.ShouldBeEquivalentTo(new[] { nameof(Example.RequiredOptionalBoolAllowsUnspecified) });
    }
}