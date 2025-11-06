using Microsoft.AspNetCore.Mvc;
using OptionalValues;

namespace OptionalValues.OpenApi.TestApp.Endpoints;

public static class References
{
    public static void MapReferences(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("/references");
        group.MapPost("body/baseline", (ReferencesBodyBaseline body) => TypedResults.Ok());
        group.MapPost("body/optional", (ReferencesBodyOptional body) => TypedResults.Ok());
    }

    public class ReferencesBodyOptional
    {
        public OptionalValue<ReferenceAOptional> A { get; set; }
    }

    public class ReferencesBodyBaseline
    {
        public ReferenceA A { get; set; } = null!;
    }

    // Baseline nested models (A -> B -> C -> A circular)
    public class ReferenceA
    {
        public string Name { get; set; } = null!;
        public ReferenceB B { get; set; } = null!;
    }

    public class ReferenceB
    {
        public string Description { get; set; } = null!;
        public ReferenceC C { get; set; } = null!;
    }

    public class ReferenceC
    {
        public int Value { get; set; }
        public ReferenceA? Parent { get; set; } // circular reference back to A
    }

    // Optional versions of the nested models
    public class ReferenceAOptional
    {
        public OptionalValue<string> Name { get; set; }
        public OptionalValue<ReferenceBOptional> B { get; set; }
    }

    public class ReferenceBOptional
    {
        public OptionalValue<string> Description { get; set; }
        public OptionalValue<ReferenceCOptional> C { get; set; }
    }

    public class ReferenceCOptional
    {
        public OptionalValue<int> Value { get; set; }
        public OptionalValue<ReferenceAOptional> Parent { get; set; } // circular back to A (nullable)
    }
}