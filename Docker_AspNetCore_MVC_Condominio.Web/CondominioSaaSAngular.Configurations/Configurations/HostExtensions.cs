using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CondominioSaaSAngular.Configurations.Configurations;

public static class HostExtensions
{
    public static IHostBuilder AddAppLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();

            if (context.HostingEnvironment.IsDevelopment())
                logging.SetMinimumLevel(LogLevel.Debug);
            else
                logging.SetMinimumLevel(LogLevel.Information);
        });
    }
}