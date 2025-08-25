using System.ComponentModel.DataAnnotations;

using Shouldly;

namespace OptionalValues.DataAnnotations.Tests;

public class SpecifiedAttributeTest
{
    private class Example
    {
        [Specified]
        public OptionalValue<bool?> OptionalBool { get; set; }
    }

    [Fact]
    public void Should_Error_For_Unspecified()
    {
        var model = new Example
        {
            OptionalBool = OptionalValue<bool?>.Unspecified,
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeFalse();
        results.Count.ShouldBe(1);
        results[0].MemberNames.ShouldBeEquivalentTo(new[] { nameof(Example.OptionalBool) });
    }

    [Fact]
    public void Should_Allow_Null()
    {
        var model = new Example
        {
            OptionalBool = null,
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true).ShouldBeTrue();
    }
}
