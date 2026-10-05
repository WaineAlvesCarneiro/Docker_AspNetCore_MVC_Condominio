using CondominioSaaSAngular.Application.Features.Moradores.Commands.Create;
using CondominioSaaSAngular.Application.Features.Moradores.Commands.Delete;
using CondominioSaaSAngular.Application.Features.Moradores.Commands.Update;
using CondominioSaaSAngular.Application.Features.Moradores.Queries.GetAll;
using CondominioSaaSAngular.Application.Features.Moradores.Queries.GetAllPaged;
using CondominioSaaSAngular.Application.Features.Moradores.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CondominioSaaSAngular.Application.Helpers;

namespace CondominioSaaSAngular.API.Endpoints;

public static class MoradorEndpoints
{
    public static void MapMoradorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/Morador")
            .WithTags("Morador");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct, long? empresaId) =>
        {
            var result = await mediator.Send(new GetAllQueryMorador(EmpresaId: empresaId == null ? 0 : Convert.ToInt64(empresaId)), ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico, Porteiro" });

        group.MapGet("/paginado", async (IMediator mediator, int page, int pageSize, string? sortBy, string? direction, long? empresaId, string? nome, CancellationToken ct) =>
        {
            var query = new GetAllPagedQueryMorador(
                Page: page == 0 ? 1 : page,
                PageSize: pageSize == 0 ? 5 : pageSize,
                SortBy: sortBy ?? "Id",
                Direction: direction ?? "ASC",
                EmpresaId: empresaId ?? null,
                Nome: nome ?? "");

            var result = await mediator.Send(query, ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico, Porteiro" });

        group.MapGet("/{id}", async (long id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetByIdQueryMorador(id), ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.NotFound(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico, Porteiro" });

        group.MapPost("/", async (CreateCommandMorador command, ClaimsPrincipal user, IMediator mediator, CancellationToken ct) =>
        {
            command.EmpresaId = user.GetEmpresaId();

            var result = await mediator.Send(command, ct);

            if (!result.Sucesso)
                return Results.BadRequest(new { sucesso = false, erro = result.Mensagem });

            return Results.Created($"/Morador/{result.Dados!.Id}", new { sucesso = true, dados = result.Dados });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico" });

        group.MapPut("/{id}", async (long id, UpdateCommandMorador command, ClaimsPrincipal user, IMediator mediator, CancellationToken ct) =>
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
            var result = await mediator.Send(new DeleteCommandMorador(id), ct);

            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Sindico" });
    }
}
