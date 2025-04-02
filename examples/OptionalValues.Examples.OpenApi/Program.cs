using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

using OptionalValues;
using OptionalValues.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add OptionalValue support to the JSON serializer
builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    jsonOptions.SerializerOptions.AddOptionalValueSupport();

    // Note: This is not required when using AddOptionalValueSupport()
    // but it is recommended to enable this always so that the serializer throws when deserializing null values into non-nullable properties.
    jsonOptions.SerializerOptions.RespectNullableAnnotations = true;
});

builder.Services.AddOpenApi(x =>
{
    x.AddDocumentTransformer(new RemoveAddSchemaEndpointsOpenApiDocumentTrasformer());
});
// builder.Services.AddTransient<IApiDescriptionProvider>(_ => new AddSchemaApiDescriptionProvider(typeof(Address)));
// builder.Services.AddTransient<IApiDescriptionProvider>(_ => new AddSchemaApiDescriptionProvider(typeof(Person)));
builder.Services.AddTransient<IApiDescriptionProvider>(_ => new AddWrappedSchemasApiDescriptionProvider());
// Add OptionalValue support to OpenApi
// This is just an helper method that adds a schema transformer to all the OpenApiOptions (when using multiple OpenApi documents)
// You can also add the transformer manually in AddOpenApi with options.AddSchemaTransformer<OptionalValueSchemaTransformer>();
builder.Services.AddOpenApiOptionalValueSupport();

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.MapOpenApi();

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

public class AddWrappedSchemasApiDescriptionProvider : IApiDescriptionProvider
{
    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        var results = context.Results.ToList();
        var checkedTypes = new HashSet<Type>();
        var typesToCheck = new Queue<Type>();
        var typesToAdd = new List<Type>();

        foreach(var result in results)
        {
            foreach(var parameter in result.ParameterDescriptions)
            {
                typesToCheck.Enqueue(parameter.ParameterDescriptor.ParameterType);
            }

            foreach(var responseType in result.SupportedResponseTypes)
            {
                if(responseType.Type is not null)
                {
                    typesToCheck.Enqueue(responseType.Type);
                }
            }
        }

        while (typesToCheck.TryDequeue(out Type typeToCheck))
        {
            if(checkedTypes.Contains(typeToCheck))
            {
                continue;
            }

            checkedTypes.Add(typeToCheck);

            if(typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == typeof(OptionalValue<>))
            {
                typesToAdd.Add(typeToCheck.GetGenericArguments()[0]);
            }

            foreach(var property in typeToCheck.GetProperties())
            {
                typesToCheck.Enqueue(property.PropertyType);
            }
        }

        foreach(var typeToAdd in typesToAdd)
        {
            context.Results.Add(new ApiDescription
            {
                HttpMethod = "GET",
                RelativePath = "/add-schema/" + typeToAdd.FullName,
                Properties = { {"type", typeToAdd } },
                ActionDescriptor = new ActionDescriptor
                {
                    DisplayName = "Add Schema " + typeToAdd.FullName,
                    EndpointMetadata = new List<object>
                    {
                        new TagsAttribute("SCHEMA_FAKE_ENDPOINT")
                    }
                },
                SupportedResponseTypes =
                {
                    new ApiResponseType()
                    {
                        StatusCode = 200,
                        Type = typeToAdd,
                        ApiResponseFormats =
                        {
                            new ApiResponseFormat
                            {
                                MediaType = "application/json"
                            }
                        }
                    }
                }
            });
        }
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context) { }

    public int Order { get; }
}

public class AddSchemaApiDescriptionProvider : IApiDescriptionProvider
{
    private readonly Type _type;

    public AddSchemaApiDescriptionProvider(Type type)
    {
        _type = type;
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        context.Results.Add(new ApiDescription
        {
            HttpMethod = "GET",
            RelativePath = "/add-schema/" + _type.FullName,
            Properties = { {"type", _type } },
            ActionDescriptor = new ActionDescriptor
            {
                DisplayName = "Add Schema " + _type.FullName,
                EndpointMetadata = new List<object>
                {
                    new TagsAttribute("SCHEMA_FAKE_ENDPOINT")
                }
            },
            SupportedResponseTypes =
            {
                new ApiResponseType()
                {
                    StatusCode = 200,
                    Type = _type,
                    ApiResponseFormats =
                    {
                        new ApiResponseFormat
                        {
                            MediaType = "application/json"
                        }
                    }
                }
            }
        });
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context) { }

    public int Order { get; }
}

public class RemoveAddSchemaEndpointsOpenApiDocumentTrasformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        foreach(var pathToRemove in document.Paths.Keys.Where(k => k.StartsWith("/add-schema/")).ToList())
        {
            document.Paths.Remove(pathToRemove);
        }

        return Task.CompletedTask;
    }
}