using invenpro.auth.common.Secrets;
using invenpro.auth.common.Settings;
using invenpro.auth.repository;
using invenpro.auth.repository.Interceptors;
using invenpro.auth.secretmanager.Service;
using invenpro.auth.secretmanager.Service.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace invenpro.auth.api.DesignTime;

public sealed class AuthServiceDbContextFactory : IDesignTimeDbContextFactory<AuthServiceDbContext>
{
    public AuthServiceDbContext CreateDbContext(string[] args)
    {
        // 1) Cargar configuración (appsettings + env)
        ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

        configurationBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        IConfigurationRoot configuration = configurationBuilder.Build();

        // 2) Bind de Settings necesarios para Secret Manager
        SecretManagerSettings secretManagerSettings = new SecretManagerSettings();
        configuration.GetSection("SecretManagerSettings").Bind(secretManagerSettings);

        EnvironmentSettings environmentSettings = new EnvironmentSettings();
        configuration.GetSection("EnvironmentSettings").Bind(environmentSettings);

        IOptions<SecretManagerSettings> secretOptions =
            Options.Create(secretManagerSettings);

        IOptions<EnvironmentSettings> environmentOptions =
            Options.Create(environmentSettings);

        // 3) Logger básico para SecretManagerService y el interceptor
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            // Si quieres ver logs en migraciones, descomenta:
            // builder.AddConsole();
        });

        ILogger<SecretManagerService> secretLogger =
            loggerFactory.CreateLogger<SecretManagerService>();

        ILogger<RepositoryTimingInterceptor> interceptorLogger =
            loggerFactory.CreateLogger<RepositoryTimingInterceptor>();

        // 4) Instanciar el SecretManagerService igual que en runtime
        ISecretManagerService secretManagerService =
            new SecretManagerService(secretLogger, secretOptions, environmentOptions);

        string? arn = secretManagerSettings.ArnAuthService;
        if (string.IsNullOrWhiteSpace(arn))
        {
            throw new InvalidOperationException(
                "SecretManagerSettings:ArnAuthService no está configurado en appsettings / env vars.");
        }

        // 5) Obtener secretos de DB desde AWS / LocalStack
        DBSecrets? dbSecrets = Task.Run(async () =>
        {
            return await secretManagerService.GetSecret<DBSecrets>(arn);
        }).GetAwaiter().GetResult();

        if (dbSecrets is null)
        {
            throw new InvalidOperationException(
                "No se pudieron obtener los secretos de base de datos desde Secrets Manager.");
        }

        // 6) Obtener nombre de DB desde ContextSettings
        string? dbAuth = configuration.GetValue<string>("ContextSettings:AuthDb");
        if (string.IsNullOrWhiteSpace(dbAuth))
        {
            throw new InvalidOperationException(
                "ContextSettings:AuthDb no está configurado en appsettings / env vars.");
        }

        string conexionBase =
            $"server={dbSecrets.Host};port={dbSecrets.Port};user={dbSecrets.Username};password='{dbSecrets.Password}'";

        string connectionString = $"{conexionBase};database='{dbAuth}'";

        // 7) Construir DbContextOptions manualmente
        DbContextOptionsBuilder<AuthServiceDbContext> optionsBuilder =
            new DbContextOptionsBuilder<AuthServiceDbContext>();

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        RepositoryTimingInterceptor interceptor =
            new RepositoryTimingInterceptor(interceptorLogger);

        AuthServiceDbContext context =
            new AuthServiceDbContext(optionsBuilder.Options, interceptor);

        return context;
    }
}