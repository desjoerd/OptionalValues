#if NET9_0_OR_GREATER

using System.Text.Json;

using Shouldly;

namespace OptionalValues.Tests.Json;

public class RespectNullableAnnotationsTest
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        RespectNullableAnnotations = true,
    }.AddOptionalValueSupport();

    private class OptionalValueModel
    {
        public OptionalValue<string> NotNullable { get; init; }

        public OptionalValue<string?> Nullable { get; init; }
    }

    private class ReferenceModel
    {
        public string NotNullable { get; init; } = null!;

        public string? Nullable { get; init; }
    }

    [Fact]
    public void ShouldThrowBecauseNullOnNotNullable()
    {
        var json = """
                   {
                       "NotNullable":null,
                   }
                   """;

        Func<OptionalValueModel?> act = () => JsonSerializer.Deserialize<OptionalValueModel>(json, Options);
        Func<ReferenceModel?> actReference = () => JsonSerializer.Deserialize<ReferenceModel>(json, Options);

        act.ShouldThrow<JsonException>();
        actReference.ShouldThrow<JsonException>();
    }

    [Fact]
    public void ShouldNotThrowBecauseNullOnNullable()
    {
        var json = """
                   {
                       "NotNullable":"Some Value",
                       "Nullable":null
                   }
                   """;

        OptionalValueModel? model = JsonSerializer.Deserialize<OptionalValueModel>(json, Options);
        ReferenceModel? modelReference = JsonSerializer.Deserialize<ReferenceModel>(json, Options);

        model!.NotNullable.SpecifiedValue.ShouldBe("Some Value");
        model.Nullable.SpecifiedValue.ShouldBeNull();

        modelReference!.NotNullable.ShouldBe("Some Value");
        modelReference.Nullable.ShouldBeNull();
    }

    [Fact]
    public void ShouldNotThrowForUnspecified()
    {
        var json = "{}";

        OptionalValueModel? model = JsonSerializer.Deserialize<OptionalValueModel>(json, Options);
        ReferenceModel? modelReference = JsonSerializer.Deserialize<ReferenceModel>(json, Options);

        model!.NotNullable.IsSpecified.ShouldBeFalse();
        model.Nullable.IsSpecified.ShouldBeFalse();

        modelReference!.NotNullable.ShouldBeNull();
        modelReference.Nullable.ShouldBeNull();
    }
}

#endif
