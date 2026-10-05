using CondominioSaaSAngular.Domain.Repositories;
using CondominioSaaSAngular.Domain.Repositories.Auth;
using CondominioSaaSAngular.Infrastructure.Repositories;
using CondominioSaaSAngular.Infrastructure.Repositories.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CondominioSaaSAngular.Configurations.Configs;

public static class RepositoriesConfig
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.TryAddScoped<IAuthUserRepository, AuthUserRepository>();
        services.TryAddScoped<IEmpresaRepository, EmpresaRepository>();
        services.TryAddScoped<IImovelRepository, ImovelRepository>();
        services.TryAddScoped<IMoradorRepository, MoradorRepository>();

        return services;
    }
}