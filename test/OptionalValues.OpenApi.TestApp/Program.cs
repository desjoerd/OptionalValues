using System.Text.Json.Serialization;

using OptionalValues;
using OptionalValues.OpenApi;
using OptionalValues.OpenApi.TestApp.Endpoints;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddOptionalValueSupport();
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
});
builder.Services.AddOpenApi(options =>
{
    options.AddOptionalValueSupport();
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPrimitives();
app.MapReferences();

app.Run();

public partial class Program
{
}