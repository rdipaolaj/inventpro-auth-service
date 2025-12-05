using invenpro.auth.common.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace invenpro.auth.common.Services;

public static class ServicesConfiguration
{
    public static IServiceCollection AddCommonServicesConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IContextAccessorService, ContextAccessorService>();
        return services;
    }
}