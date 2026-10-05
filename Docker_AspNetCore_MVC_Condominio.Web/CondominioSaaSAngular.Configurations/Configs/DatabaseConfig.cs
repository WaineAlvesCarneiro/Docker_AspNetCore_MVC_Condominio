using CondominioSaaSAngular.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CondominioSaaSAngular.Configurations.Configs;

public static class DatabaseConfig
{
    private const int DefaultCommandTimeout = 60;
    private const int DefaultMaxRetryCount = 5;
    private static readonly TimeSpan DefaultMaxRetryDelay = TimeSpan.FromSeconds(30);

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                     .CreateLogger("DatabaseConfig");

            LogEnvironment(logger, configuration);

            ConfigureSqlServer(options, connectionString, configuration);
        });

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("ApplicationDbContext")
            ?? throw new InvalidOperationException(
                "Connection string 'ApplicationDbContext' não encontrada. " +
                "Defina a variável de ambiente 'ConnectionStrings__ApplicationDbContext'.");
    }

    private static void LogEnvironment(ILogger logger, IConfiguration configuration)
    {
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Unknown";
        logger.LogDebug("Configurando conexão com banco para ambiente: {Environment}", environment);
    }

    private static void ConfigureSqlServer(DbContextOptionsBuilder options, string connectionString, IConfiguration configuration)
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("CondominioSaaSAngular.Infrastructure");
            sqlOptions.CommandTimeout(DefaultCommandTimeout);
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: DefaultMaxRetryCount,
                maxRetryDelay: DefaultMaxRetryDelay,
                errorNumbersToAdd: null);
        });

        if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    }
}
