using Shouldly;

namespace OptionalValues.Tests;

public class OptionalValueTest
{
    public class Constructor : OptionalValueTest
    {
        [Fact]
        public void DefaultConstructor_Gives_Unspecified()
        {
            var sut = new OptionalValue<string?>();

            sut.IsSpecified.ShouldBeFalse();
            sut.ShouldBe(default(OptionalValue<string?>));
            sut.ShouldBe(OptionalValue<string?>.Unspecified);
        }

        [Fact]
        public void Constructor_With_Value_Gives_Value()
        {
            var sut = new OptionalValue<string?>("Value");

            sut.IsSpecified.ShouldBeTrue();
            sut.Value.ShouldBe("Value");
        }
    }

    public class IsSpecified : OptionalValueTest
    {
        [Fact]
        public void Should_Be_False_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.IsSpecified.ShouldBeFalse();
        }

        [Fact]
        public void Should_Be_False_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            sut.IsSpecified.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Value")]
        public void Should_Be_True_For(string? value)
        {
            OptionalValue<string?> sut = value;

            sut.IsSpecified.ShouldBeTrue();
        }
    }

    public class Value : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.Value.ShouldBe("Value");
        }

        [Fact]
        public void Should_Be_Null_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.Value!.ShouldBeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            sut.Value!.ShouldBeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Null()
        {
            OptionalValue<string?> sut = null;

            sut.Value!.ShouldBeNull();
        }
    }

    public class SpecifiedValue : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.SpecifiedValue.ShouldBe("Value");
        }

        [Fact]
        public void Should_Be_Value_Even_When_Null()
        {
            OptionalValue<string?> sut = null;

            sut.SpecifiedValue.ShouldBe(null);
        }

        [Fact]
        public void Should_Throw_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            Should.Throw<InvalidOperationException>(() => sut.SpecifiedValue)
                .Message.ShouldBe("Value is unspecified.");
        }
    }

    public class ImplicitConversionToOptionalValue : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.Value.ShouldBe("Value");
        }
    }

    public class ImplicitConversionToT : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            var value = (string?)sut;

            value.ShouldBe("Value");
        }

        [Fact]
        public void Should_Be_Null_When_Default()
        {
            OptionalValue<string?> sut = default;

            string? value = sut;

            value!.ShouldBeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Unspecified()
        {
            OptionalValue<string?> sut = OptionalValue<string?>.Unspecified;

            string? value = sut;

            value!.ShouldBeNull();
        }

        [Fact]
        public void Should_Be_Null_When_Null()
        {
            OptionalValue<string?> sut = null;

            string? value = sut;

            value!.ShouldBeNull();
        }
    }

    public class EqualsTest : OptionalValueTest
    {
        public class EqualsOptionalValue : EqualsTest
        {
            [Fact]
            public void Should_Be_Equal_When_Same_Value()
            {
                OptionalValue<string?> left = "Value";
                OptionalValue<string?> right = "Value";

                left.Equals(right).ShouldBeTrue();
                right.Equals(left).ShouldBeTrue();
            }

            [Fact]
            public void Should_Be_Equal_When_Same_Null()
            {
                OptionalValue<string?> left = null;
                OptionalValue<string?> right = null;

                left.Equals(right).ShouldBeTrue();
                right.Equals(left).ShouldBeTrue();
            }

            [Fact]
            public void Should_Be_Equal_When_Same_Unspecified()
            {
                OptionalValue<string?> left = OptionalValue<string?>.Unspecified;
                OptionalValue<string?> right = OptionalValue<string?>.Unspecified;

                left.Equals(right).ShouldBeTrue();
                right.Equals(left).ShouldBeTrue();
            }

            [Fact]
            public void Should_Be_Equal_When_Same_Default()
            {
                OptionalValue<string?> left = default;
                OptionalValue<string?> right = default;

                left.Equals(right).ShouldBeTrue();
                right.Equals(left).ShouldBeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Different_Value()
            {
                OptionalValue<string?> left = "Value1";
                OptionalValue<string?> right = "Value2";

                left.Equals(right).ShouldBeFalse();
                right.Equals(left).ShouldBeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_One_Unspecified()
            {
                OptionalValue<string?> left = OptionalValue<string?>.Unspecified;
                OptionalValue<string?> right = "Value2";

                left.Equals(right).ShouldBeFalse();
                right.Equals(left).ShouldBeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_One_Default()
            {
                OptionalValue<string?> left = default;
                OptionalValue<string?> right = "Value2";

                left.Equals(right).ShouldBeFalse();
                right.Equals(left).ShouldBeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_One_Null()
            {
                OptionalValue<string?> left = default;
                OptionalValue<string?> right = null;

                left.Equals(right).ShouldBeFalse();
                right.Equals(left).ShouldBeFalse();
            }
        }

        public class EqualsObject : EqualsTest
        {
            [Theory]
            [InlineData(null, null)]
            [InlineData("Value", "Value")]
            public void Should_Be_Equal_When_Same(string? left, string? right)
            {
                object sut = (OptionalValue<string?>)left;

                sut.Equals(right).ShouldBeTrue();
            }

            [Theory]
            [InlineData(null, "Value")]
            [InlineData("Value", null)]
            [InlineData("Value1", "Value2")]
            public void Should_Not_Be_Equal_When_Different(string? left, string? right)
            {
                object sut = (OptionalValue<string?>)left;

                sut.Equals(right).ShouldBeFalse();
            }

            [Fact]
            public void Null_Should_Not_Be_Equal_To_Unspecified()
            {
                new OptionalValue<string?>(null).Equals(OptionalValue<string?>.Unspecified)
                    .ShouldBeFalse();

                OptionalValue<string?>.Unspecified.Equals((object?)null)
                    .ShouldBeFalse();
            }
        }
    }

    public class GetSpecifiedValueOrDefault : OptionalValueTest
    {
        [Fact]
        public void Should_Be_Value()
        {
            OptionalValue<string?> sut = "Value";

            sut.GetSpecifiedValueOrDefault().ShouldBe("Value");
        }

        [Fact]
        public void Should_Be_Default_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.GetSpecifiedValueOrDefault().ShouldBe(null);
        }

        [Fact]
        public void Should_Be_Provided_Default_When_Default()
        {
            OptionalValue<string?> sut = default;

            sut.GetSpecifiedValueOrDefault("Default").ShouldBe("Default");
        }

        [Fact]
        public void Should_Be_Value_Even_When_Provided_Default()
        {
            OptionalValue<string?> sut = "Value";

            sut.GetSpecifiedValueOrDefault("Default").ShouldBe("Value");
        }

        [Fact]
        public void Should_Be_Value_Even_When_Null()
        {
            OptionalValue<string?> sut = null;

            sut.GetSpecifiedValueOrDefault("Default").ShouldBe(null);
        }
    }
}
