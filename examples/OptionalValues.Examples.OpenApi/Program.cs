using System.Text.Json.Serialization;

using OptionalValues;
using OptionalValues.DataAnnotations;
using OptionalValues.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddOptionalValueSupport();
});

// Add OptionalValue support to the JSON serializer
builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    jsonOptions.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
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

class NullableProperty
{
    [Specified]
    public OptionalValue<string?> Name { get; init; }
}

/// <summary>
/// This is the main example company model.
/// </summary>
class Company
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Name of the company.
    /// </summary>
    [RequiredValue]
    public OptionalValue<string> Name { get; init; }

    [OptionalLength(0, 50)]
    public OptionalValue<string?> Summary { get; init; }

    /// <summary>
    /// The contact person for the company.
    /// </summary>
    [Specified]
    public OptionalValue<Person?> Contact { get; init; }
}

/// <summary>
/// This is a person.
/// </summary>
class Person
{
    /// <summary>
    /// The full name of the person.
    /// </summary>
    [Specified]
    public OptionalValue<string?> Name { get; init; } = "John Doe";

    [OptionalRange(0, 120)]
    public OptionalValue<int> Age { get; init; }

    /// <summary>
    /// Contact address for the person.
    /// </summary>
    public OptionalValue<Address> Address { get; init; }

    /// <summary>
    /// Billing address for the person.
    /// </summary>
    public OptionalValue<Address?> BillingAddress { get; init; }
}

/// <summary>
/// A mailing address.
/// </summary>
class Address
{
    [Specified]
    public OptionalValue<string?> Street { get; init; }

    public OptionalValue<string> City { get; init; }

    [OptionalRegularExpression("^[a-zA-Z ]+$")]
    public OptionalValue<string> State { get; init; }

    public OptionalValue<string?> Zip { get; init; }
}

/// <summary>
/// Entry point for the OptionalValues.Examples.OpenApi application.
/// </summary>
public partial class Program {
}