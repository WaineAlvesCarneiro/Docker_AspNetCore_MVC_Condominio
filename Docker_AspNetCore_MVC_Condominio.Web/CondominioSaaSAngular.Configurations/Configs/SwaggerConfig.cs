using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CondominioSaaSAngular.Configurations.Configs;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerAndSecurity(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Condomínio",
                Version = "v1",
                Description = "API do Condomínio"
            });

            OpenApiSecurityScheme securityScheme = GerarSecurityScheme();

            opt.AddSecurityDefinition("Bearer", securityScheme);
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        });

        return services;
    }

    private static OpenApiSecurityScheme GerarSecurityScheme()
    {
        return new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando Bearer.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        };
    }
}