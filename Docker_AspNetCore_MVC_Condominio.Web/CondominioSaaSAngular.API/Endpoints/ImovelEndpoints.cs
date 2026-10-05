using CondominioSaaSAngular.Application.Features.Imoveis.Commands.Create;
using CondominioSaaSAngular.Application.Features.Imoveis.Commands.Delete;
using CondominioSaaSAngular.Application.Features.Imoveis.Commands.Update;
using CondominioSaaSAngular.Application.Features.Imoveis.Queries.GetAll;
using CondominioSaaSAngular.Application.Features.Imoveis.Queries.GetAllPaged;
using CondominioSaaSAngular.Application.Features.Imoveis.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CondominioSaaSAngular.Application.Helpers;

namespace CondominioSaaSAngular.API.Endpoints;

public static class ImovelEndpoints
{
    public static void MapImovelEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/Imovel")
            .WithTags("Imovel");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct, long? empresaId) =>
        {
            var result = await mediator.Send(new GetAllQueryImovel(EmpresaId: empresaId == null ? 0 : Convert.ToInt64(empresaId)), ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico, Porteiro" });

        group.MapGet("/paginado", async (IMediator mediator, int page, int pageSize, string? sortBy, string? direction, long? empresaId, string? bloco, string? apartamento, CancellationToken ct) =>
        {
            var query = new GetAllPagedQueryImovel(
                    Page: page == 0 ? 1 : page,
                    PageSize: pageSize == 0 ? 5 : pageSize,
                    SortBy: sortBy ?? "Id",
                    Direction: direction ?? "ASC",
                    EmpresaId: empresaId,
                    Bloco: bloco ?? "",
                    Apartamento: apartamento ?? ""
                );

            var result = await mediator.Send(query, ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico, Porteiro" });

        group.MapGet("/{id}", async (long id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetByIdQueryImovel(id), ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.NotFound(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico, Porteiro" });

        group.MapPost("/", async (CreateCommandImovel command, ClaimsPrincipal user, IMediator mediator, CancellationToken ct) =>
        {
            command.EmpresaId = user.GetEmpresaId();

            var result = await mediator.Send(command, ct);

            if (!result.Sucesso)
                return Results.BadRequest(new { sucesso = false, erro = result.Mensagem });

            return Results.Created($"/Imovel/{result.Dados!.Id}", new { sucesso = true, dados = result.Dados });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico" });

        group.MapPut("/{id}", async (long id, UpdateCommandImovel command, ClaimsPrincipal user, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.Id)
                return Results.BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");

            command.EmpresaId = user.GetEmpresaId();

            var result = await mediator.Send(command, ct);

            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico" });

        group.MapDelete("/{id}", async (long id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteCommandImovel(id), ct);

            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico" });
    }
}
