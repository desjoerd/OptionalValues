using System.Text.Json;

using Shouldly;

namespace OptionalValues.Tests.Json;

public class RecordTest
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        .AddOptionalValueSupport();

    private record RecordModel(OptionalValue<string> Name);

    [Fact]
    public void CanSerializeEmpty()
    {
        var model = new RecordModel(default);
        var result = JsonSerializer.Serialize(model, Options);

        Assert.Equal("{}", result);
    }

    [Fact]
    public void CanDeserializeEmpty()
    {
        var json = "{}";

        RecordModel? result = JsonSerializer.Deserialize<RecordModel>(json, Options);
        Assert.NotNull(result);
        result.Name.IsSpecified.ShouldBeFalse();
    }
}