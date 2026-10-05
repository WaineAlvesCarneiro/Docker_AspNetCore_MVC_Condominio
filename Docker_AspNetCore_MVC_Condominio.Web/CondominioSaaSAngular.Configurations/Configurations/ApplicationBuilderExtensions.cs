using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CondominioSaaSAngular.Configurations.Configurations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Condomínio v1.0.0");
            c.DocumentTitle = "API Asp Net Core - Swagger";
        });

        app.UseCors("AllowFrontend");
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}