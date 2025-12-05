using invenpro.auth.api;
using invenpro.auth.infraestructure.Modules;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfigurationSettings(builder)
    .AddCustomMvc()
    .AddHttpContextAccessor()
    .AddMediatRAssemblyConfiguration()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddGlobalExceptionHandler()
    .AddCustomServicesConfiguration()
    .AddApiVersioningConfiguration()
    .AddSecretManagerConfiguration(builder)
    .AddMapsterConfiguration()
    .AddDatabaseConfiguration(builder)
    .AddJwtAuthentication(builder.Configuration, builder);

WebApplication? app = builder.Build();

app.UseSwagger();
app.ConfigurationSwagger();

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseCustomMiddlewares();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();