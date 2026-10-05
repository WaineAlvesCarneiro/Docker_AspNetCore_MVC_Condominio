using CondominioSaaSAngular.Application.Behaviors;
using CondominioSaaSAngular.Application.Features.Auth;
using CondominioSaaSAngular.Application.Features.Auth.Commands.Create;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CondominioSaaSAngular.Configurations.Configs;

public static class MediatRConfig
{
    public static IServiceCollection AddMediatRAndValidators(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AuthLoginQuery).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(CreateCommandValidatorAuthUser).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("Processing {RequestName} at {Timestamp}", requestName, DateTime.UtcNow);

            var response = await next();

            _logger.LogInformation("Completed {RequestName} at {Timestamp}", requestName, DateTime.UtcNow);

            return response;
        }
    }
}