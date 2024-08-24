using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enums;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Endpoints;

public static class VeiculosEndpoints
{
    public static WebApplication MapVeiculosEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("/veiculos")
            .WithTags("Veiculos")
            .RequireAuthorization();

        var endpointsWithId = endpoints.MapGroup("/{id:int}");

        endpoints.MapPost("", async (
            [FromBody] VeiculoDto veiculoDto,
            IVeiculoServico veiculoServico) =>
        {
            var errosDeValidacao = ValidaVeiculoDto(veiculoDto);

            if (errosDeValidacao.Mensagens.Count > 0)
            {
                return Results.BadRequest(errosDeValidacao);
            }

            var veiculo = new Veiculo
            {
                Nome = veiculoDto.Nome,
                Marca = veiculoDto.Marca,
                Ano = veiculoDto.Ano
            };

            await veiculoServico.Incluir(veiculo);

            return Results.CreatedAtRoute("GetVeiculo", new { id = veiculo.Id }, veiculo);
        })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ErrosDeValidacao>(StatusCodes.Status400BadRequest)
            .Produces<Veiculo>(StatusCodes.Status201Created);

        endpoints.MapGet("", async (
            [FromQuery] int? pagina,
            [FromQuery] string? nome,
            [FromQuery] string? marca,
            IVeiculoServico veiculoServico) =>
        {
            var veiculos = await veiculoServico.Todos(pagina, nome, marca);

            return Results.Ok(veiculos);
        })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<List<Veiculo>>(StatusCodes.Status200OK);

        endpointsWithId.MapGet("", async (
            [FromRoute] int id,
            IVeiculoServico veiculoServico) =>
        {
            var veiculo = await veiculoServico.BuscaPorId(id);

            return veiculo is null ? Results.NotFound() : Results.Ok(veiculo);
        })
            .WithName("GetVeiculo")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<Veiculo>(StatusCodes.Status200OK);

        endpointsWithId.MapPut("", async (
            [FromRoute] int id,
            [FromBody] VeiculoDto veiculoDto,
            IVeiculoServico veiculoServico) =>
        {
            var validacao = ValidaVeiculoDto(veiculoDto);

            if (validacao.Mensagens.Count > 0)
            {
                return Results.BadRequest(validacao);
            }

            var veiculo = await veiculoServico.BuscaPorId(id);

            if (veiculo is null)
            {
                return Results.NotFound();
            }

            veiculo.Nome = veiculoDto.Nome;
            veiculo.Marca = veiculoDto.Marca;
            veiculo.Ano = veiculoDto.Ano;

            await veiculoServico.Atualizar(veiculo);

            return Results.Ok(veiculo);
        })
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(Perfil.Adm) })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ErrosDeValidacao>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<Veiculo>(StatusCodes.Status200OK);

        endpointsWithId.MapDelete("", async (
            [FromRoute] int id,
            IVeiculoServico veiculoServico) =>
        {
            var veiculo = await veiculoServico.BuscaPorId(id);

            if (veiculo is null)
            {
                return Results.NotFound();
            }

            await veiculoServico.Apagar(veiculo);

            return Results.NoContent();
        })
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(Perfil.Adm) })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent);

        return app;
    }

    private static ErrosDeValidacao ValidaVeiculoDto(VeiculoDto veiculoDto)
    {
        List<string> mensagens = [];

        if (string.IsNullOrWhiteSpace(veiculoDto.Nome))
        {
            mensagens.Add("O nome não pode ficar em branco");
        }

        if (string.IsNullOrWhiteSpace(veiculoDto.Marca))
        {
            mensagens.Add("A marca não pode ficar em branco");
        }

        if (veiculoDto.Ano < 1950)
        {
            mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");
        }

        return new ErrosDeValidacao(mensagens);
    }
}
