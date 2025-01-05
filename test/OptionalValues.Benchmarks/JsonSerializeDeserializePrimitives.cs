using System.Text.Json;
using System.Text.Json.Serialization;

using BenchmarkDotNet.Attributes;

using OptionalValues.Benchmarks.TestObjects;

namespace OptionalValues.Benchmarks;

[MemoryDiagnoser]
public class JsonSerializeDeserializePrimitives
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };
    private static readonly JsonSerializerOptions OptionalValueSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    }.AddOptionalValueSupport();

    private static readonly JsonSerializerOptions OptionalValueSerializerWithSourceGeneratorOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
    {
        TypeInfoResolver = TestObjects.PrimitivesJsonSerializationContext.Default,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    }.AddOptionalValueSupport();

    private static readonly PrimitiveModel DefaultModelInstance = new PrimitiveModel();
    private static readonly OptionalValueModel OptionalValueModelInstance = new OptionalValueModel();

    [Benchmark(Baseline = true)]
    public PrimitiveModel SerializeDeserializePrimitiveModel()
    {
        var json = JsonSerializer.Serialize(DefaultModelInstance, DefaultSerializerOptions);
        return JsonSerializer.Deserialize<PrimitiveModel>(json, DefaultSerializerOptions)!;
    }

    [Benchmark]
    public OptionalValueModel SerializeDeserializeOptionalValueModel()
    {
        var json = JsonSerializer.Serialize(OptionalValueModelInstance, OptionalValueSerializerOptions);
        return JsonSerializer.Deserialize<OptionalValueModel>(json, OptionalValueSerializerOptions)!;
    }

    [Benchmark]
    public PrimitiveModel SerializeDeserializePrimitiveModelWithSourceGenerator()
    {
        var json = JsonSerializer.Serialize(DefaultModelInstance, OptionalValueSerializerWithSourceGeneratorOptions);
        return JsonSerializer.Deserialize<PrimitiveModel>(json, OptionalValueSerializerWithSourceGeneratorOptions)!;
    }

    [Benchmark]
    public OptionalValueModel SerializeDeserializeOptionalValueModelWithSourceGenerator()
    {
        var json = JsonSerializer.Serialize(OptionalValueModelInstance, OptionalValueSerializerWithSourceGeneratorOptions);
        return JsonSerializer.Deserialize<OptionalValueModel>(json, OptionalValueSerializerWithSourceGeneratorOptions)!;
    }
}