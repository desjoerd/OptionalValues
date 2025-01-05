using System.Text.Json.Serialization;

namespace OptionalValues.Benchmarks.TestObjects;

[JsonSerializable(typeof(PrimitiveModel))]
[JsonSerializable(typeof(OptionalValueModel))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
public partial class PrimitivesJsonSerializationContext : JsonSerializerContext
{
}