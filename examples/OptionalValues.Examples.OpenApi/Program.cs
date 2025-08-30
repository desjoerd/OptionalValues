using System.Text.Json.Serialization.Metadata;

using Microsoft.OpenApi.Models;

using OptionalValues;
using OptionalValues.DataAnnotations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    var originalCreateSchemaReferenceId = options.CreateSchemaReferenceId;

    options.CreateSchemaReferenceId = (jsonTypeInfo) =>
    {
        if (OptionalValue.IsOptionalValueType(jsonTypeInfo.Type))
        {
            var underlyingType = OptionalValue.GetUnderlyingType(jsonTypeInfo.Type);
            var underlyingJsonTypeInfo = JsonTypeInfo.CreateJsonTypeInfo(underlyingType, jsonTypeInfo.Options);

            return originalCreateSchemaReferenceId(underlyingJsonTypeInfo);
        }

        return originalCreateSchemaReferenceId(jsonTypeInfo);
    };

    options.AddSchemaTransformer(async (schema, context, cancel) =>
    {
        if (context.JsonPropertyInfo == null)
        {
            return;
        }

        if (!OptionalValue.IsOptionalValueType(context.JsonPropertyInfo.PropertyType))
        {
            return;
        }

        Type underlyingType = OptionalValue.GetUnderlyingType(context.JsonPropertyInfo.PropertyType);

        OpenApiSchema underlyingSchema = await context.GetOrCreateSchemaAsync(underlyingType);
        schema.Type = underlyingSchema.Type;
        schema.Format = underlyingSchema.Format;
        schema.Properties = underlyingSchema.Properties;
        schema.Items = underlyingSchema.Items;
    });
});

// Add OptionalValue support to the JSON serializer
builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    jsonOptions.SerializerOptions.AddOptionalValueSupport();
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// This is an example of how to use OptionalValues with NSwag
// It also shows what the JSON Serializer will do with the OptionalValues
// You can play with the values in the Company object to see how the JSON Serializer will handle them
// Play around by omitting properties or whole objects
app.MapPost("/company", (Company company) => company)
    .WithName("Example Post")
    .WithDescription("This directly returns the posted object")
    .WithTags("Example");

app.Run();

// Some Example models to play with OptionalValues
class Company
{
    /// <summary>
    /// The unique identifier for the company
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// The name of the company
    /// </summary>
    [OptionalLength(0, 50)]
    public OptionalValue<string?> Summary { get; init; }

    /// <summary>
    /// The Contact person for the company
    /// </summary>
    public OptionalValue<Person?> Contact { get; init; }
}

/// <summary>
/// A person
/// </summary>
class Person
{
    public OptionalValue<string?> Name { get; init; } = "John Doe";

    [OptionalRange(0, 120)]
    public OptionalValue<int> Age { get; init; }

    /// <summary>
    /// Address of the person.
    /// </summary>
    public Address Address { get; init; }

    /// <summary>
    /// Address2 of the person.
    /// </summary>
    public Address Address2 { get; init; }
}

class Address
{
    /// <summary>
    /// The street address.
    /// </summary>
    public OptionalValue<string> Street { get; init; }

    public OptionalValue<string> City { get; init; }

    [OptionalRegularExpression("^[a-zA-Z ]+$")]
    public OptionalValue<string> State { get; init; }

    public OptionalValue<string?> Zip { get; init; }
}


/// <summary>
/// This is a simple program to demonstrate how to use OptionalValues with OpenAPI
/// </summary>
partial class Program;