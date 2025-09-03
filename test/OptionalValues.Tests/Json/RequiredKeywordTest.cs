using System.Text.Json;

using Shouldly;

namespace OptionalValues.Tests.Json;

public class RequiredKeywordTest
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        .AddOptionalValueSupport();

    public class RequiredKeywordClassModel
    {
        public required OptionalValue<string> NotRequired { get; init; }
    }

    public record RequiredKeywordRecordModel
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

        RequiredKeywordClassModel? requiredKeywordClassModel = JsonSerializer.Deserialize<RequiredKeywordClassModel>(json, Options);
        RequiredKeywordRecordModel? requiredKeywordRecordModel = JsonSerializer.Deserialize<RequiredKeywordRecordModel>(json, Options);
        Func<ReferenceModel?> actReference = () => JsonSerializer.Deserialize<ReferenceModel>(json, Options);

        requiredKeywordClassModel!.NotRequired.IsSpecified.ShouldBeFalse();
        requiredKeywordRecordModel!.NotRequired.IsSpecified.ShouldBeFalse();
        actReference.ShouldThrow<JsonException>();
    }
}
