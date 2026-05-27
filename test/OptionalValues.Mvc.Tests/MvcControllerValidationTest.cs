using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

namespace OptionalValues.Mvc.Tests;

public class MvcControllerValidationTest
{
    [Fact]
    public void AddOptionalValueSupport_ShouldConfigureOptionalValueValidationMetadata()
    {
        ServiceProvider services = new ServiceCollection()
            .AddControllers(options => options.AddOptionalValueSupport())
            .Services
            .BuildServiceProvider();

        var metadataProvider = services.GetRequiredService<IModelMetadataProvider>();
        var metadata = metadataProvider.GetMetadataForType(typeof(OptionalValue<MvcControllerValidationChildModel>));
        var value = metadata.Properties[nameof(OptionalValue<MvcControllerValidationChildModel>.Value)];
        var specifiedValue = metadata.Properties[nameof(OptionalValue<MvcControllerValidationChildModel>.SpecifiedValue)];
        var isSpecified = metadata.Properties[nameof(OptionalValue<MvcControllerValidationChildModel>.IsSpecified)];

        metadata.ValidateChildren.ShouldBeTrue();
        value.ShouldNotBeNull();
        value.PropertyValidationFilter.ShouldBeNull();
        specifiedValue.ShouldNotBeNull();
        specifiedValue.PropertyValidationFilter.ShouldNotBeNull();
        isSpecified.ShouldNotBeNull();
        isSpecified.PropertyValidationFilter.ShouldNotBeNull();
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
