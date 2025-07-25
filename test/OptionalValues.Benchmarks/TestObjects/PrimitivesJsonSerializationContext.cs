using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptionalValues.Benchmarks.TestObjects;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(PrimitiveModel))]
[JsonSerializable(typeof(OptionalValueModel))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
public partial class PrimitivesJsonSerializationContext : JsonSerializerContext
{
}