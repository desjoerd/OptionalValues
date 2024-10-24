using FluentAssertions;

namespace OptionalValues.Tests;

public class OptionalValueTest
{
    public class Constructor : OptionalValueTest
    {
        [Fact]
        public void DefaultConstructor_Gives_Unspecified()
        {
            var sut = new OptionalValue<string?>();

            sut.IsSpecified.Should().BeFalse();
            sut.Should().Be(default(OptionalValue<string?>));
            sut.Should().Be(OptionalValue<string>.Unspecified);
        }

        [Fact]
        public void Constructor_With_Value_Gives_Value()
        {
            var sut = new OptionalValue<string?>("Value");

            sut.IsSpecified.Should().BeTrue();
            sut.Value.Should().Be("Value");
        }
    }

    public class IsSpecified : OptionalValueTest
    {
        [Fact]
        public void Should_Be_False_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.IsSpecified.Should().BeFalse();
        }

        [Fact]
        public void Should_Be_False_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            sut.IsSpecified.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Value")]
        public void Should_Be_True_For(string? value)
        {
            OptionalValue<string?> sut = value;

            sut.IsSpecified.Should().BeTrue();
        }
    }

    public class Value : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.Value.Should().Be("Value");
        }

        [Fact]
        public void Should_Be_Null_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.Value.Should().BeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            sut.Value.Should().BeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Null()
        {
            OptionalValue<string?> sut = null;

            sut.Value.Should().BeNull();
        }
    }

    public class SpecifiedValue : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.SpecifiedValue.Should().Be("Value");
        }

        [Fact]
        public void Should_Be_Value_Even_When_Null()
        {
            OptionalValue<string?> sut = null;

            sut.SpecifiedValue.Should().Be(null);
        }

        [Fact]
        public void Should_Throw_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            sut.Invoking(x => x.SpecifiedValue)
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Value is unspecified.");
        }
    }

    public class ImplicitConversionToOptionalValue : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.Value.Should().Be("Value");
        }
    }

    public class ImplicitConversionToT : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            var value = (string?)sut;

            value.Should().Be("Value");
        }

        [Fact]
        public void Should_Be_Null_When_Default()
        {
            OptionalValue<string?> sut = default;

            string? value = sut;

            value.Should().BeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            string? value = sut;

            value.Should().BeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Null()
        {
            OptionalValue<string?> sut = null;

            string? value = sut;

            value.Should().BeNull();
        }
    }

    public class EqualityOptionalValue : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Equal_When_Same_Value()
        {
            OptionalValue<string?> left = "Value";
            OptionalValue<string?> right = "Value";

            left.Should().Be(right);
        }

        [Fact]
        public void Should_Be_Equal_When_Same_Unspecified()
        {
            OptionalValue<string?> left = OptionalValue<string?>.Unspecified;
            OptionalValue<string?> right = OptionalValue<string?>.Unspecified;

            left.Should().Be(right);
        }

        [Fact]
        public void Should_Be_Equal_When_Same_Default()
        {
            OptionalValue<string?> left = default;
            OptionalValue<string?> right = default;

            left.Should().Be(right);
        }

        [Fact]
        public void Should_Not_Be_Equal_When_Different_Value()
        {
            OptionalValue<string?> left = "Value1";
            OptionalValue<string?> right = "Value2";

            left.Should().NotBe(right);
        }

        [Fact]
        public void Should_Not_Be_Equal_When_One_Unspecified()
        {
            OptionalValue<string?> left = OptionalValue<string?>.Unspecified;
            OptionalValue<string?> right = "Value2";

            left.Should().NotBe(right);
        }

        [Fact]
        public void Should_Not_Be_Equal_When_One_Default()
        {
            OptionalValue<string?> left = default;
            OptionalValue<string?> right = "Value2";

            left.Should().NotBe(right);
        }

        [Fact]
        public void Should_Not_Be_Equal_When_One_Null()
        {
            OptionalValue<string?> left = default;
            OptionalValue<string?> right = null;

            left.Should().NotBe(right);
        }
    }

    public class GetSpecifiedValueOrDefault : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.GetSpecifiedValueOrDefault().Should().Be("Value");
        }

        [Fact]
        public void Should_Be_Default_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.GetSpecifiedValueOrDefault().Should().Be(null);
        }

        [Fact]
        public void Should_Be_Provided_Default_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.GetSpecifiedValueOrDefault("Default").Should().Be("Default");
        }

        [Fact]
        public void Should_Be_Value_Even_When_Provided_Default()
        {
            OptionalValue<string?> sut = "Value";

            sut.GetSpecifiedValueOrDefault("Default").Should().Be("Value");
        }

        [Fact]
        public void Should_Be_Value_Even_When_Null()
        {
            OptionalValue<string?> sut = null;

            sut.GetSpecifiedValueOrDefault("Default").Should().Be(null);
        }
    }
}
