using CondominioSaaSAngular.Application.Features.Empresas.Commands.Create;
using CondominioSaaSAngular.Application.Features.Empresas.Commands.Delete;
using CondominioSaaSAngular.Application.Features.Empresas.Commands.Update;
using CondominioSaaSAngular.Application.Features.Empresas.Queries.GetAll;
using CondominioSaaSAngular.Application.Features.Empresas.Queries.GetAllPaged;
using CondominioSaaSAngular.Application.Features.Empresas.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CondominioSaaSAngular.API.Endpoints;

public static class EmpresaEndpoints
{
    public static void MapEmpresaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/Empresa")
            .WithTags("Empresa");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct, long? empresaId) =>
        {
            var result = await mediator.Send(new GetAllQueryEmpresa(EmpresaId: empresaId == null ? 0 : Convert.ToInt64(empresaId)), ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Suporte, Sindico" });

        group.MapGet("/paginado", async (IMediator mediator, int page, int pageSize, string? sortBy, string? direction, string? razaoSocial, string? cnpj, CancellationToken ct) =>
        {
            var query = new GetAllPagedQueryEmpresa(
                Page: page == 0 ? 1 : page,
                PageSize: pageSize == 0 ? 5 : pageSize,
                SortBy: sortBy ?? "Id",
                Direction: direction ?? "ASC",
                RazaoSocial: razaoSocial,
                Cnpj: cnpj);

            var result = await mediator.Send(query, ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapGet("/{id}", async (long id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetByIdQueryEmpresa(id), ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.NotFound(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapPost("/", async (CreateCommandEmpresa command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);

            if (!result.Sucesso)
                return Results.BadRequest(new { sucesso = false, erro = result.Mensagem });

            return Results.Created($"/Empresa/{result.Dados!.Id}", new { sucesso = true, dados = result.Dados });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapPut("/{id}", async (long id, UpdateCommandEmpresa command, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");
            }

            var result = await mediator.Send(command, ct);

            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapDelete("/{id}", async (long id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteCommandEmpresa(id), ct);

            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });
    }
}
