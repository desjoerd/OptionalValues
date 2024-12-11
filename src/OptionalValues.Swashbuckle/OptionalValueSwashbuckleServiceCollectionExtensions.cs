using System.Text.Json;

using Microsoft.Extensions.Options;

using OptionalValues;
using OptionalValues.Swashbuckle;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service Collection Extensions to add support for <see cref="OptionalValue{T}"/> to Swashbuckle.
/// </summary>
public static class OptionalValueSwashbuckleServiceCollectionExtensions
{
    /// <summary>
    /// Add support for <see cref="OptionalValue{T}"/> to Swashbuckle.
    ///
    /// This method will add a custom <see cref="ISerializerDataContractResolver"/> to the service collection.
    /// </summary>
    /// <remarks>It is preferred to call this method after <c>services.AddSwaggerGen()</c> because it will try to wrap the existing <see cref="ISerializerDataContractResolver"/>.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> to allow for chaining.</returns>
    public static IServiceCollection AddSwaggerGenOptionalValueSupport(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var replacedExisting = false;
        for(var i = services.Count - 1; i >= 0; i--)
        {
            if (services[i].ServiceType != typeof(ISerializerDataContractResolver))
            {
                continue;
            }

            Func<IServiceProvider, object>? innerFactory = services[i].ImplementationFactory;
            if (innerFactory != null)
            {
                services[i] = ServiceDescriptor.Singleton<ISerializerDataContractResolver>(sp =>
                {
                    var inner = (ISerializerDataContractResolver)innerFactory(sp);
                    return new OptionalValueDataContractResolver(inner);
                });

                replacedExisting = true;
            }
        }

        if (!replacedExisting)
        {
            services.AddSingleton<ISerializerDataContractResolver>(sp =>
            {
                JsonSerializerOptions serializerOptions = sp.GetService<IOptions<AspNetCore.Mvc.JsonOptions>>()?.Value.JsonSerializerOptions
                                                          ?? sp.GetService<IOptions<AspNetCore.Http.Json.JsonOptions>>()?.Value.SerializerOptions
                                                          ?? JsonSerializerOptions.Default;

                return new OptionalValueDataContractResolver(serializerOptions);
            });
        }

        return services;
    }
}