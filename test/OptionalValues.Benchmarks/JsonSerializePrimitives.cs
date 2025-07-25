using System.Text.Json;

using BenchmarkDotNet.Attributes;

using OptionalValues.Benchmarks.TestObjects;

namespace OptionalValues.Benchmarks;

[MemoryDiagnoser]
public class JsonSerializePrimitives
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = JsonSerializerOptions.Default;
    private static readonly JsonSerializerOptions OptionalValueSerializerOptions = JsonSerializerOptions.Default
        .WithOptionalValueSupport();

    private static readonly JsonSerializerOptions OptionalValueSerializerWithSourceGeneratorOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
    {
        TypeInfoResolver = PrimitivesJsonSerializationContext.Default
    }.AddOptionalValueSupport();

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