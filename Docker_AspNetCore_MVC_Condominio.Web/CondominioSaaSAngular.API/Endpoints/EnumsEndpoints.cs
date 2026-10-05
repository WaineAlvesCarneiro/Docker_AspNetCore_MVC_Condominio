using CondominioSaaSAngular.Domain.Enums;

namespace CondominioSaaSAngular.API.Endpoints;

public static class EnumsEndpoints
{
    public static void MapEnumsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/Enums")
            .WithTags("Enums")
            .RequireAuthorization("AdminPolicy");

        group.MapGet("/tipo-condominio", () => Results.Ok(GetEnumOptions<TipoCondominio>()))
            .WithName("GetTipoCondominio");

        group.MapGet("/tipo-role", () => Results.Ok(GetEnumOptions<TipoRole>()))
            .WithName("GetTipoRole");

        group.MapGet("/tipo-user-ativo", () => Results.Ok(GetEnumOptions<TipoUserAtivo>()))
            .WithName("GetTipoUserAtivo");

        group.MapGet("/tipo-empresa-ativo", () => Results.Ok(GetEnumOptions<TipoEmpresaAtivo>()))
            .WithName("GetTipoEmpresaAtivo");
    }

    private static IEnumerable<object> GetEnumOptions<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new
            {
                Value = Convert.ToInt32(e),
                Label = e.ToString()
            });
    }
}