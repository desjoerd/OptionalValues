using OptionalValues;
using OptionalValues.DataAnnotations;
using OptionalValues.NSwag;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "OptionalValues.Examples.NSwag";
    options.PostProcess = doc => doc.Generator = null;

    // Add OptionalValue support to NSwag
    options.SchemaSettings.TypeMappers.Add(new OptionalValueTypeMapper());
});

// Add OptionalValue support to the JSON serializer
builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    jsonOptions.SerializerOptions.AddOptionalValueSupport();
});

WebApplication app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();

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
    public Guid Id { get; init; } = Guid.NewGuid();

    [OptionalRequired]
    public OptionalValue<string> Name { get; init; }

    [OptionalLength(0, 50)]
    public OptionalValue<string?> Summary { get; init; }

    public OptionalValue<Person?> Contact { get; init; }
}

class Person
{
    public OptionalValue<string?> Name { get; init; } = "John Doe";

    [OptionalRange(0, 120)]
    public OptionalValue<int> Age { get; init; }

    public OptionalValue<Address> Address { get; init; }
}

class Address
{
    [OptionalSpecified]
    public OptionalValue<string> Street { get; init; }

    public OptionalValue<string> City { get; init; }

    [OptionalRegularExpression("^[a-zA-Z ]+$")]
    public OptionalValue<string> State { get; init; }

    public OptionalValue<string?> Zip { get; init; }
}
