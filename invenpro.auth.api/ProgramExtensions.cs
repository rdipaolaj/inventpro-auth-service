using Asp.Versioning;
using invenpro.auth.api.Configuration;
using invenpro.auth.common.Responses;
using invenpro.auth.common.Secrets;
using invenpro.auth.common.Settings;
using invenpro.auth.dto.Mapster;
using invenpro.auth.infraestructure.Middlewares;
using invenpro.auth.repository;
using invenpro.auth.repository.Interceptors;
using invenpro.auth.secretmanager.Service;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace invenpro.auth.api;

public static class ProgramExtensions
{
    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ApiResponseWrapperFilter>();
        });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        services.AddOptions();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }

    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }

    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Services.Configure<SecretManagerSettings>(builder.Configuration.GetSection("SecretManagerSettings"));
        builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
        builder.Services.Configure<Observability>(builder.Configuration.GetSection("Observability")); // configurar aun en el appsetting
        builder.Services.Configure<EnvironmentSettings>(builder.Configuration.GetSection("EnvironmentSettings"));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        return services;
    }

    public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader());
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        return services;
    }

    public static WebApplication ConfigurationSwagger(this WebApplication app)
    {
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();

            foreach (var groupName in descriptions.Select(x => x.GroupName))
            {
                options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
            }
        });

        return app;
    }

    public static IServiceCollection AddSecretManagerConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue<bool>("SecretManagerSettings:Local"))
        {
            string? arnAuthService = builder.Configuration.GetValue<string>("SecretManagerSettings:ArnAuthService");
            string? arnJwtService = builder.Configuration.GetValue<string>("SecretManagerSettings:ArnJwtService");

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ISecretManagerService secretManagerService = (ISecretManagerService)serviceProvider.GetService(typeof(ISecretManagerService));

            Task.Run(async () =>
            {
                JwtSecrets? jwtSecrets = await secretManagerService.GetSecret<JwtSecrets>(arnJwtService);

                if (jwtSecrets is not null)
                {
                    services.Configure<JwtSettings>(options =>
                    {
                        options.Secret = jwtSecrets.Secret;
                    });
                }

            }).GetAwaiter().GetResult();
        }

        return services;
    }

    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig mapsterConfiguration = MapsterConfiguration.Configuration();

        services.AddSingleton(mapsterConfiguration);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseMiddleware<ExecutionTimeMiddleware>()
            /*.UseMiddleware<CustomExceptionMiddleware>()*/;

        return app;
    }

    private static IServiceCollection AddFormatConeccionString(this IServiceCollection services, DBSecrets dbSecrets, WebApplicationBuilder builder)
    {
        string? dbAuth = builder.Configuration.GetValue<string>("ContextSettings:AuthDb");

        string conexionBase = $"server={dbSecrets.Host};port={dbSecrets.Port};user={dbSecrets.Username};password='{dbSecrets.Password}'";
        string connectionString = $"{conexionBase};database='{dbAuth}'";

        services.AddDbContext<AuthServiceDbContext>((serviceProvider, options) =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            RepositoryTimingInterceptor interceptor = serviceProvider.GetRequiredService<RepositoryTimingInterceptor>();
            options.AddInterceptors(interceptor);
        }, ServiceLifetime.Scoped);

        return services;
    }

    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue<bool>("SecretManagerSettings:Local"))
        {
            string? arn = builder.Configuration.GetValue<string>("SecretManagerSettings:ArnAuthService");

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ISecretManagerService? secretManagerService = (ISecretManagerService)serviceProvider.GetService(typeof(ISecretManagerService));

            DBSecrets dbSecrets = Task.Run(async () =>
            {
                return await secretManagerService.GetSecret<DBSecrets>(arn);
            }).GetAwaiter().GetResult();

            services.AddFormatConeccionString(dbSecrets, builder);
        }

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
    {
        string? jwtSecrets = builder.Configuration.GetValue<string>("SecretManagerSettings:ArnJwtService");
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        ISecretManagerService secretManagerService = (ISecretManagerService)serviceProvider.GetService(typeof(ISecretManagerService));
        JwtSecrets? ciamSecrets = secretManagerService?.GetSecret<JwtSecrets>(jwtSecrets ?? string.Empty).GetAwaiter().GetResult();

        string? secretKey = ciamSecrets.Secret;
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Jwt:Secret no está configurado (se esperaba cargarlo desde Secrets Manager).");
        }

        services
            .AddAuthentication(options =>
            {
                // 🔹 Esquema por defecto para autenticar y para los challenges ([Authorize])
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // para local
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

                    // Si quieres validar issuer/audience, ponlos en el secret o en appsettings:
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuer = !string.IsNullOrWhiteSpace(configuration["Jwt:Issuer"]),
                    ValidateAudience = !string.IsNullOrWhiteSpace(configuration["Jwt:Audience"]),

                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}