using CondominioSaaSAngular.Infrastructure.Messaging;
using CondominioSaaSAngular.Infrastructure.Messaging.Configurations;
using CondominioSaaSAngular.Infrastructure.Services;
using CondominioSaaSAngular.Configurations.ServicesJWT;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CondominioSaaSAngular.Domain.Interfaces;

namespace CondominioSaaSAngular.Configurations.Configs;

public static class InfrastructureConfig
{
    public static IServiceCollection AddRabbitMQEmailTokenServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));

        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILoggerFactory>()
                             .CreateLogger("InfrastructureConfig");

        var rabbitSettings = configuration.GetSection("RabbitMq").Get<RabbitMqSettings>();

        if (rabbitSettings != null) logger.LogInformation("RabbitMQ configurado. Host: {Host}, User: {User}", rabbitSettings.Host, rabbitSettings.UserName);
        else logger.LogWarning("RabbitMQ Settings não encontrados. Usando defaults.");

        services.AddSingleton<IMensageriaService, RabbitMQService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddSingleton<TokenService>();

        return services;
    }
}
