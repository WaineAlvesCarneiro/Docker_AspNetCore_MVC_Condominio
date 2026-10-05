using CondominioSaaSAngular.Domain.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CondominioSaaSAngular.Configurations.Configs;

public static class SecurityConfig
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = GetJwtKey(configuration);

        services.AddAuthentication(options =>
        {
            ConfigureAuthenticationSchemes(options);
        })
        .AddJwtBearer(options =>
        {
            ConfigureJwtBearerOptions(options, configuration, key);
        });
    }

    private static byte[] GetJwtKey(IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            throw new InvalidOperationException("JWT Key inválida ou muito curta (mínimo 32 caracteres).");

        return Encoding.ASCII.GetBytes(jwtKey);
    }

    private static void ConfigureAuthenticationSchemes(AuthenticationOptions options)
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }

    private static void ConfigureJwtBearerOptions(JwtBearerOptions options, IConfiguration configuration, byte[] key)
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = BuildTokenValidationParameters(configuration, key);
        options.Events = BuildJwtBearerEvents();
    }

    private static TokenValidationParameters BuildTokenValidationParameters(IConfiguration configuration, byte[] key)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };
    }

    private static JwtBearerEvents BuildJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                logger.LogWarning("Falha na autenticação: {Exception}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                logger.LogInformation("Token validado para usuário: {User}", context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            }
        };
    }

    public static void AddAppAuthorizationPolicies(this IServiceCollection services)
    {
        var rolesPermitidas = new[] { "Suporte", "Sindico", "Porteiro" };

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.RequireRole(rolesPermitidas);
                policy.RequireClaim(AuthClaims.PrimeiroAcesso, AuthClaims.FalseValue);
                policy.RequireClaim(AuthClaims.StatusAtivo, AuthClaims.StatusAtivoValue);
                policy.RequireClaim(AuthClaims.EmpresaAtiva, AuthClaims.StatusAtivoValue);
                policy.RequireAssertion(context => context.User.Identity?.IsAuthenticated == true);
            });

            options.AddPolicy("PermitirTrocaSenha", policy =>
            {
                policy.RequireRole(rolesPermitidas);
                policy.RequireClaim(AuthClaims.PrimeiroAcesso, AuthClaims.TrueValue);
                policy.RequireClaim(AuthClaims.StatusAtivo, AuthClaims.StatusAtivoValue);
                policy.RequireClaim(AuthClaims.EmpresaAtiva, AuthClaims.StatusAtivoValue);
            });
        });
    }
}
