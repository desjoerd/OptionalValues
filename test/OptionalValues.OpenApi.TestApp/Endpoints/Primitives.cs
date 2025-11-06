using Microsoft.AspNetCore.Mvc;

namespace OptionalValues.OpenApi.TestApp.Endpoints;

public static class Primitives
{
    public static void MapPrimitives(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("/primitives");
        group.MapPost("body/baseline", (PrimitivesBodyBaseline body) => TypedResults.Ok());
        group.MapPost("body/optional", (PrimitivesBodyOptional body) => TypedResults.Ok());
    }

    public class PrimitivesBodyOptional
    {
        public OptionalValue<int> Int { get; set; }
        public OptionalValue<int?> NullableInt { get; set; }
        public OptionalValue<uint> UInt { get; set; }
        public OptionalValue<uint?> NullableUInt { get; set; }
        public OptionalValue<double> Double { get; set; }
        public OptionalValue<double?> NullableDouble { get; set; }
        public OptionalValue<bool> Bool { get; set; }
        public OptionalValue<bool?> NullableBool { get; set; }
        public OptionalValue<string> String { get; set; }
        public OptionalValue<string?> NullableString { get; set; }
        public OptionalValue<Guid> Guid { get; set; }
        public OptionalValue<Guid?> NullableGuid { get; set; }
        public OptionalValue<DateTime> DateTime { get; set; }
        public OptionalValue<DateTime?> NullableDateTime { get; set; }
        public OptionalValue<DateOnly> DateOnly { get; set; }
        public OptionalValue<DateOnly?> NullableDateOnly { get; set; }
        public OptionalValue<DateTimeOffset> DateTimeOffset { get; set; }
        public OptionalValue<DateTimeOffset?> NullableDateTimeOffset { get; set; }
        public OptionalValue<TimeOnly> TimeOnly { get; set; }
        public OptionalValue<TimeOnly?> NullableTimeOnly { get; set; }
        public OptionalValue<TimeSpan> TimeSpan { get; set; }
        public OptionalValue<TimeSpan?> NullableTimeSpan { get; set; }
    }

    public class PrimitivesBodyBaseline
    {
        public int Int { get; set; }
        public int? NullableInt { get; set; }
        public uint UInt { get; set; }
        public uint? NullableUInt { get; set; }
        public double Double { get; set; }
        public double? NullableDouble { get; set; }
        public bool Bool { get; set; }
        public bool? NullableBool { get; set; }
        public string String { get; set; } = null!;
        public string? NullableString { get; set; }
        public Guid Guid { get; set; }
        public Guid? NullableGuid { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? NullableDateTime { get; set; }
        public DateOnly DateOnly { get; set; }
        public DateOnly? NullableDateOnly { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public DateTimeOffset? NullableDateTimeOffset { get; set; }
        public TimeOnly TimeOnly { get; set; }
        public TimeOnly? NullableTimeOnly { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public TimeSpan? NullableTimeSpan { get; set; }
    }
}