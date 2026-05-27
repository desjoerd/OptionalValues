using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

namespace OptionalValues.DataAnnotations.Tests;

public class MvcControllerValidationTest
{
    [Fact]
    public void SpecifiedValue_ShouldBeIgnoredByMvcValidation()
    {
        ServiceProvider services = new ServiceCollection()
            .AddControllers()
            .Services
            .BuildServiceProvider();

        var metadataProvider = services.GetRequiredService<IModelMetadataProvider>();
        var metadata = metadataProvider.GetMetadataForType(typeof(OptionalValue<MvcControllerValidationChildModel>));

        var specifiedValue = metadata.Properties[nameof(OptionalValue<MvcControllerValidationChildModel>.SpecifiedValue)];

        specifiedValue.ShouldNotBeNull();
        specifiedValue.PropertyValidationFilter.ShouldNotBeNull();
    }
}

public class MvcControllerValidationChildModel
{
    public string? Value { get; init; }
}
