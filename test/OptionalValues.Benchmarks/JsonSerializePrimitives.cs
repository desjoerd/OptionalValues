using System.Text.Json;
using System.Text.Json.Serialization;

using BenchmarkDotNet.Attributes;

namespace OptionalValues.Benchmarks;

[MemoryDiagnoser]
public class JsonSerializePrimitives
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = JsonSerializerOptions.Default;
    private static readonly JsonSerializerOptions OptionalValueSerializerOptions = JsonSerializerOptions.Default
        .WithOptionalValueSupport();

    private static readonly JsonSerializerOptions OptionalValueSerializerWithSourceGeneratorOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
    {
        TypeInfoResolver = SerializeSimpleJsonJsonSerializationContext.Default
    }.AddOptionalValueSupport();

    public class PrimitiveModel
    {
        public int Age { get; set; } = 42;
        public string FirstName { get; set; } = "John";
        public string? LastName { get; set; } = null;
    }

    public class OptionalValueModel
    {
        public OptionalValue<int> Age { get; set; } = 42;
        public OptionalValue<string> FirstName { get; set; } = "John";
        public OptionalValue<string> LastName { get; set; } = default;
    }

    private static readonly PrimitiveModel DefaultModelInstance = new PrimitiveModel();
    private static readonly OptionalValueModel OptionalValueModelInstance = new OptionalValueModel();

    [Benchmark(Baseline = true)]
    public string SerializePrimitiveModel()
    {
        return JsonSerializer.Serialize(DefaultModelInstance, DefaultSerializerOptions);
    }

    [Benchmark]
    public string SerializeOptionalValueModel()
    {
        return JsonSerializer.Serialize(OptionalValueModelInstance, OptionalValueSerializerOptions);
    }

    [Benchmark]
    public string SerializePrimitiveModelWithSourceGenerator()
    {
        return JsonSerializer.Serialize(DefaultModelInstance, OptionalValueSerializerWithSourceGeneratorOptions);
    }

    [Benchmark]
    public string SerializeOptionalValueModelWithSourceGenerator()
    {
        return JsonSerializer.Serialize(OptionalValueModelInstance, OptionalValueSerializerWithSourceGeneratorOptions);
    }
}

[JsonSerializable(typeof(JsonSerializePrimitives.PrimitiveModel))]
[JsonSerializable(typeof(JsonSerializePrimitives.OptionalValueModel))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
public partial class SerializeSimpleJsonJsonSerializationContext : JsonSerializerContext
{
}