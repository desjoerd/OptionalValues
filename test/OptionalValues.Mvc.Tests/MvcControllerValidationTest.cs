using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

namespace OptionalValues.Mvc.Tests;

public class MvcControllerValidationTest
{
    [Fact]
    public void AddOptionalValuesMvc_ShouldDisableChildValidationForOptionalValueTypes()
    {
        ServiceProvider services = new ServiceCollection()
            .AddControllers(options => options.AddOptionalValuesMvc())
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
    public void AddOptionalValuesMvc_ShouldOnlyRegisterProviderOnce()
    {
        var options = new MvcOptions();

        options.AddOptionalValuesMvc();
        options.AddOptionalValuesMvc();

        options.ModelMetadataDetailsProviders
            .OfType<OptionalValueValidationMetadataProvider>()
            .ShouldHaveSingleItem();
    }
}

public class MvcControllerValidationChildModel
{
    public string? Value { get; init; }
}
