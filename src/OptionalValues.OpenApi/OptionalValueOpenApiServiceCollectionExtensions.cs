using OptionalValues.OpenApi;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionalValueOpenApiServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApiOptionalValueSupport(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureOpenApiOptions>();

        return services;
    }
}