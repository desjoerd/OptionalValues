using System.Net;
using System.Text;

#if NET10_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
#endif
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
#if NET10_0_OR_GREATER
using Microsoft.AspNetCore.TestHost;
#endif
using Microsoft.Extensions.DependencyInjection;
#if NET10_0_OR_GREATER
using Microsoft.Extensions.Hosting;
#endif

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

#if NET10_0_OR_GREATER
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
        var content = new StringContent("{}", Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client!.PostAsync("/validation", content);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SpecifiedOptionalValue_ShouldPass_ControllerValidation()
    {
        var content = new StringContent("""{"child":{"value":"present"}}""", Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client!.PostAsync("/validation", content);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
#endif

public class MvcControllerValidationRequestModel
{
    public OptionalValue<MvcControllerValidationChildModel> Child { get; init; }
}

public class MvcControllerValidationChildModel
{
    public string? Value { get; init; }
}

#if NET10_0_OR_GREATER
[ApiController]
[Route("validation")]
public class ValidationController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(MvcControllerValidationRequestModel model) => Ok(model);
}
#endif
