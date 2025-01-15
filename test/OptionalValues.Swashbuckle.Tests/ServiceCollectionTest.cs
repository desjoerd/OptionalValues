using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace OptionalValues.Swashbuckle.Tests;

public class ServiceCollectionTest
{
    [Fact]
    public void AddSwaggerGenOptionalValueSupport_Replaces_DataContractResolver()
    {
        var services = new ServiceCollection();
        services.AddSwaggerGen();

        ServiceProvider serviceProviderWithoutOptionalValue = services.BuildServiceProvider();
        ISerializerDataContractResolver dataContractResolverWithoutOptionalValue = serviceProviderWithoutOptionalValue
            .GetRequiredService<ISerializerDataContractResolver>();
        dataContractResolverWithoutOptionalValue.ShouldBeOfType<JsonSerializerDataContractResolver>();

        // Add OptionalValue support
        services.AddSwaggerGenOptionalValueSupport();

        ServiceProvider serviceProviderWithOptionalValue = services.BuildServiceProvider();

        ISerializerDataContractResolver dataContractResolverWithOptionalValue = serviceProviderWithOptionalValue
            .GetRequiredService<ISerializerDataContractResolver>();

        dataContractResolverWithOptionalValue.ShouldBeOfType<OptionalValueDataContractResolver>();
    }

    [Fact]
    public void Adding_SwaggerGenOptionalValueSupport_Before_AddSwaggerGen_Gives_The_OptionalValueDataContractResolver()
    {
        var services = new ServiceCollection();

        services.AddSwaggerGenOptionalValueSupport();
        services.AddSwaggerGen();

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        ISerializerDataContractResolver dataContractResolver = serviceProvider.GetRequiredService<ISerializerDataContractResolver>();
        dataContractResolver.ShouldBeOfType<OptionalValueDataContractResolver>();
    }

    [Fact]
    public void AddSwaggerGenOptionalValueSupport_Is_Registered_With_Working_Factory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSwaggerGen();

        // Act
        services.AddSwaggerGenOptionalValueSupport();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        ISerializerDataContractResolver dataContractResolver = serviceProvider.GetRequiredService<ISerializerDataContractResolver>();
        dataContractResolver.ShouldBeOfType<OptionalValueDataContractResolver>();

        dataContractResolver.GetDataContractForType(typeof(ExampleType)).ShouldNotBeNull();
    }

    private class ExampleType
    {
        public OptionalValue<int> OptionalInt { get; set; }

        public int Int { get; set; }
    }
}
