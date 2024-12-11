using System.ComponentModel.DataAnnotations;

using Microsoft.OpenApi.Models;

using OptionalValues;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OptionalValues.Examples.Swashbuckle", Version = "1.0" });
});
// Add OptionalValue support to Swashbuckle
builder.Services.AddSwaggerGenOptionalValueSupport();

// Add OptionalValue support to the JSON serializer
builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    jsonOptions.SerializerOptions.AddOptionalValueSupport();
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// This is an example of how to use OptionalValues with Swashbuckle
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

    [Length(0, 50)]
    [MaxLength(60)]
    public OptionalValue<string?> Summary { get; init; }

    public OptionalValue<Person?> Contact { get; init; }
}

public class Person
{
    public OptionalValue<string?> Name { get; init; } = "John Doe";

    [Range(0, 120)]
    public OptionalValue<int> Age { get; init; }

    public OptionalValue<Address> Address { get; init; }
}

public class Address
{
    public OptionalValue<string> Street { get; init; }

    public OptionalValue<string> City { get; init; }

    public OptionalValue<string> State { get; init; }

    public OptionalValue<string?> Zip { get; init; }
}