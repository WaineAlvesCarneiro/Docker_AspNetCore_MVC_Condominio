using CondominioSaaSAngular.API.Endpoints;
using CondominioSaaSAngular.API.Endpoints.Auth;
using CondominioSaaSAngular.Configurations.Configs;
using CondominioSaaSAngular.Configurations.Configurations;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddAppLogging();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddMediatRAndValidators();
builder.Services.AddRepositories();
builder.Services.AddRabbitMQEmailTokenServices(builder.Configuration);
builder.Services.AddAppCorsPolicy(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppAuthorizationPolicies();
builder.Services.AddHealthChecks();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ConfigureJsonDefaults();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerAndSecurity();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("healthy"));

await app.ApplyMigrationsWithRetryAsync();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

app.UseAppMiddleware(app.Environment);
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapEnumsEndpoints();
app.MapAuthEndpoints();
app.MapEmpresaEndpoints();
app.MapImovelEndpoints();
app.MapMoradorEndpoints();

app.Run("http://0.0.0.0:8083");

public partial class Program { }
