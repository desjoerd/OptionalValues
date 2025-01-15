using System.Text.Json;

using Shouldly;

namespace OptionalValues.Tests.Json;

public class RequiredKeywordTest
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        .AddOptionalValueSupport();

    public class RequiredKeywordModel
    {
        public required OptionalValue<string> NotRequired { get; init; }
    }

    public class ReferenceModel
    {
        public required string Required { get; init; }
    }

    [Fact]
    public void ShouldNotMarkOptionalValueAsRequiredEvenWithRequiredKeyword()
    {
        var json = "{}";

        RequiredKeywordModel? requiredKeywordModel = JsonSerializer.Deserialize<RequiredKeywordModel>(json, Options);
        Func<ReferenceModel?> actReference = () => JsonSerializer.Deserialize<ReferenceModel>(json, Options);

        requiredKeywordModel!.NotRequired.IsSpecified.ShouldBeFalse();
        actReference.ShouldThrow<JsonException>();
    }
}
