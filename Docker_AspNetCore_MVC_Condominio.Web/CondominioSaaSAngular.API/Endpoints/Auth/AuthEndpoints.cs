using CondominioSaaSAngular.Application.Features.Auth;
using CondominioSaaSAngular.Application.Features.Auth.Commands.Create;
using CondominioSaaSAngular.Application.Features.Auth.Commands.DefinirSenha;
using CondominioSaaSAngular.Application.Features.Auth.Commands.Delete;
using CondominioSaaSAngular.Application.Features.Auth.Commands.Update;
using CondominioSaaSAngular.Application.Features.Auth.Queries.GetAll;
using CondominioSaaSAngular.Application.Features.Auth.Queries.GetAllPaged;
using CondominioSaaSAngular.Application.Features.Auth.Queries.GetById;
using CondominioSaaSAngular.Application.Helpers;
using CondominioSaaSAngular.Domain.Entities.Auth;
using CondominioSaaSAngular.Configurations.ServicesJWT;
using CondominioSaaSAngular.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CondominioSaaSAngular.API.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/Auth")
            .WithTags("Auth");

        group.MapPost("/login", async (AuthLoginRequest request, IMediator mediator, TokenService tokenService, CancellationToken ct) =>
        {
            var query = new AuthLoginQuery { Username = request.Username, Password = request.Password };
            var user = await mediator.Send(query, ct);

            if (user == null)
                return Results.Unauthorized();

            if (user.EmpresaAtiva != TipoEmpresaAtivo.Ativo)
                return Results.BadRequest(new { sucesso = false, erro = "O acesso para este Condomínio está suspenso. Procure o suporte." });

            if (user.Ativo != TipoUserAtivo.Ativo)
                return Results.BadRequest(new { sucesso = false, erro = "Seu usuário está inativo." });

            var token = tokenService.GenerateToken(
                user.UserName,
                (TipoRole)user.Role,
                user.EmpresaId,
                user.PrimeiroAcesso,
                (TipoUserAtivo)user.Ativo,
                (TipoEmpresaAtivo)user.EmpresaAtiva
            );

            return Results.Ok(new { token, sucesso = true, primeiroAcesso = user.PrimeiroAcesso });
        }).AllowAnonymous();

        group.MapPost("/definir-senha-permanente", async (DefinirSenhaRequest request, IMediator mediator, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var username = user.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var command = new DefinirSenhaCommand
            {
                UserName = username,
                NovaSenha = request.NovaSenha
            };

            var result = await mediator.Send(command, ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados, mensagem = result.Mensagem })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization("PermitirTrocaSenha");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAllQueryAuthUser(EmpresaId: null), ct);
            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Policy = "AdminPolicy" }).WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapGet("/paginado", async (IMediator mediator, int page, int pageSize, string? sortBy, string? direction, long? empresaId, string? userName, CancellationToken ct) =>
        {
            var query = new GetAllPagedQueryAuthUser(
                Page: page == 0 ? 1 : page,
                PageSize: pageSize == 0 ? 5 : pageSize,
                SortBy: sortBy ?? "Id",
                Direction: direction ?? "ASC",
                EmpresaId: empresaId ?? null,
                UserName: userName ?? "");

            var result = await mediator.Send(query, ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Policy = "AdminPolicy" }).WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapGet("/{id}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetByIdQueryAuthUser(id), ct);

            return result.Sucesso
                ? Results.Ok(new { sucesso = true, dados = result.Dados })
                : Results.NotFound(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Policy = "AdminPolicy" }).WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapPost("/criar-usuario", async (CreateCommandAuthUser command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);

            if (!result.Sucesso)
                return Results.BadRequest(new { sucesso = false, erro = result.Mensagem });

            return Results.Created($"/Auth/{result.Dados!.Id}", new { sucesso = true, dados = result.Dados });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Policy = "AdminPolicy" }).WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapPut("/{id}", async (Guid id, UpdateCommandAuthUser command, ClaimsPrincipal user, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.Id)
                return Results.BadRequest("O ID da URL não corresponde ao ID do corpo da requisição.");

            if (!user.IsSuporte())
                command.Role = null;

            var result = await mediator.Send(command, ct);

            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Policy = "AdminPolicy" }).WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });

        group.MapDelete("/{id}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteCommandAuthUser(id), ct);

            return result.Sucesso
                ? Results.NoContent()
                : Results.BadRequest(new { sucesso = false, erro = result.Mensagem });
        }).RequireAuthorization().WithMetadata(new AuthorizeAttribute { Policy = "AdminPolicy" }).WithMetadata(new AuthorizeAttribute { Roles = "Suporte" });
    }

    public record DefinirSenhaRequest(string NovaSenha);
}
