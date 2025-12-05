using FluentValidation;
using invenpro.auth.common.Services;
using invenpro.auth.infraestructure.Behaviors;
using invenpro.auth.infraestructure.PreProcessors;
using invenpro.auth.repository.Interceptors;
using invenpro.auth.services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace invenpro.auth.infraestructure.Modules;

public static class MediatorModule
{
    public static IServiceCollection AddMediatRAssemblyConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(requesthandler.AssemblyReference.Assembly);
            configuration.AddOpenRequestPreProcessor(typeof(CaptureHeadersPreProcessor<>));
            configuration.AddOpenRequestPreProcessor(typeof(CaptureS3InfoPreProcessor<>));
            configuration.AddOpenBehavior(typeof(TracingBehavior<,>));
            configuration.AddOpenBehavior(typeof(HeaderValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            //configuration.AddOpenRequestPostProcessor(typeof(S3PostProcessor<,>));
        });

        services.AddValidatorsFromAssembly(requestvalidator.AssemblyReference.Assembly);

        return services;
    }

    public static IServiceCollection AddCustomServicesConfiguration(this IServiceCollection services)
    {
        services.AddServicesConfiguration()
            .AddCommonServicesConfiguration();

        Assembly[] assemblies = [
            secretmanager.AssemblyReference.Assembly,
            redis.AssemblyReference.Assembly,
            application.AssemblyReference.Assembly,
            repository.AssemblyReference.Assembly,
            //encryption.AssemblyReference.Assembly
        ];

        services.AddSingleton<RepositoryTimingInterceptor>();

        services.Scan(selector => selector
            .FromAssemblies(assemblies)
            .AddClasses(publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}