using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OptionalValues.DataAnnotations;
using Shouldly;

namespace OptionalValues.Mvc.Tests;

public class MvcControllerValidationTest
{
    [Fact]
    public void AddOptionalValueSupport_ShouldDisableChildValidationForOptionalValueTypes()
    {
        ServiceProvider services = new ServiceCollection()
            .AddControllers(options => options.AddOptionalValueSupport())
            .Services
            .BuildServiceProvider();

        var metadataProvider = services.GetRequiredService<IModelMetadataProvider>();
        var metadata = metadataProvider.GetMetadataForType(typeof(OptionalValue<MvcControllerValidationChildModel>));
        var specifiedValue = metadata.Properties[nameof(OptionalValue<MvcControllerValidationChildModel>.SpecifiedValue)];

        metadata.ValidateChildren.ShouldBeFalse();
        specifiedValue.ShouldNotBeNull();
        specifiedValue.PropertyValidationFilter.ShouldBeNull();
    }

    [Fact]
    public void AddOptionalValueSupport_ShouldOnlyRegisterProviderOnce()
    {
        var options = new MvcOptions();

        options.AddOptionalValueSupport();
        options.AddOptionalValueSupport();

        options.ModelMetadataDetailsProviders
            .OfType<OptionalValueValidationMetadataProvider>()
            .ShouldHaveSingleItem();
    }
}

public class MvcControllerIntegrationValidationTest : IAsyncLifetime
{
    private WebApplication? _app;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddControllers(options => options.AddOptionalValueSupport())
            .AddApplicationPart(typeof(ValidationController).Assembly)
            .AddJsonOptions(options => options.JsonSerializerOptions.AddOptionalValueSupport());

        builder.WebHost.UseTestServer();

        _app = builder.Build();
        _app.MapControllers();

        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }

        _client?.Dispose();
    }

    [Fact]
    public async Task UnspecifiedOptionalValue_ShouldPass_ControllerValidation()
    {
        var content = new StringContent("""{"requiredField":null}""", Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client!.PostAsync("/validation", content);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SpecifiedOptionalValue_ShouldPass_ControllerValidation()
    {
        var content = new StringContent("""{"name":"short","requiredField":"present","child":{"value":"valid"}}""", Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client!.PostAsync("/validation", content);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task InvalidOptionalValueDataAnnotations_ShouldFail_ControllerValidation()
    {
        var content = new StringContent("""{"name":"toolong","requiredField":"present"}""", Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client!.PostAsync("/validation", content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        HttpValidationProblemDetails? problemDetails =
            await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        problemDetails.ShouldNotBeNull();
        problemDetails.Errors.ShouldContainKey(nameof(MvcControllerValidationRequestModel.Name));
    }

    [Fact]
    public async Task InvalidChildDataAnnotations_ShouldFail_ControllerValidation()
    {
        var content = new StringContent("""{"requiredField":"present","child":{"value":"toolong"}}""", Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client!.PostAsync("/validation", content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        HttpValidationProblemDetails? problemDetails =
            await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        problemDetails.ShouldNotBeNull();
        problemDetails.Errors.ShouldContainKey($"{nameof(MvcControllerValidationRequestModel.Child)}.{nameof(MvcControllerValidationChildModel.Value)}");
    }
}

public class MvcControllerValidationRequestModel
{
    [OptionalStringLength(5)]
    public OptionalValue<string> Name { get; init; }

    [Specified]
    public OptionalValue<string?> RequiredField { get; init; }

    public OptionalValue<MvcControllerValidationChildModel> Child { get; init; }
}

public class MvcControllerValidationChildModel
{
    [StringLength(5)]
    public string? Value { get; init; }
}

[ApiController]
[Route("validation")]
public class ValidationController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(MvcControllerValidationRequestModel model) => Ok(model);
}
