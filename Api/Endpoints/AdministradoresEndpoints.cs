using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enums;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalApi.Endpoints;

public static class AdministradoresEndpoints
{
    public static WebApplication MapAdministradoresEndpoints(
        this WebApplication app, IConfiguration configuration)
    {
        var key = configuration.GetValue<string>("Jwt:Key");
        ArgumentException.ThrowIfNullOrEmpty(key);

        var endpoints = app.MapGroup("/administradores")
            .WithTags("Administradores")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(Perfil.Adm) });

        endpoints.MapPost("/login", async (
            [FromBody] LoginDto loginDto,
            IAdministradorServico administradorServico) =>
        {
            var administrador = await administradorServico.Login(loginDto);

            if (administrador is null)
            {
                return Results.Unauthorized();
            }

            var token = GerarTokenJwt(administrador, key);

            return Results.Ok(new AdministradorLogado(administrador.Email, administrador.Perfil, token));
        })
            .AllowAnonymous()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<AdministradorLogado>(StatusCodes.Status200OK);

        endpoints.MapPost("", async (
            [FromBody] AdministradorDto administradorDto,
            IAdministradorServico administradorServico) =>
        {
            var errosDeValidacao = ValidaAdministradorDto(administradorDto);

            if (errosDeValidacao.Mensagens.Count > 0)
            {
                return Results.BadRequest(errosDeValidacao);
            }

            if (await administradorServico.Existe(administradorDto.Email))
            {
                return Results.BadRequest(new ErrosDeValidacao(["Email já cadastrado"]));
            }

            var administrador = new Administrador
            {
                Email = administradorDto.Email,
                Senha = administradorDto.Senha,
                Perfil = administradorDto.Perfil.ToString()
            };

            await administradorServico.Incluir(administrador);

            return Results.CreatedAtRoute(
                "GetAdministrador",
                new { id = administrador.Id },
                new AdministradorModelView(administrador.Id, administrador.Email, administrador.Perfil));
        })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ErrosDeValidacao>(StatusCodes.Status400BadRequest)
            .Produces<AdministradorModelView>(StatusCodes.Status201Created);

        endpoints.MapGet("", async (
            [FromQuery] int? pagina,
            IAdministradorServico administradorServico) =>
        {
            var administradores = await administradorServico.Todos(pagina);

            var administradoresModelView = administradores
                .Select(a => new AdministradorModelView(a.Id, a.Email, a.Perfil))
                .ToList();

            return Results.Ok(administradoresModelView);
        })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<List<AdministradorModelView>>(StatusCodes.Status200OK);

        endpoints.MapGet("/{id:int}", async (
            [FromRoute] int id,
            IAdministradorServico administradorServico) =>
        {
            var administrador = await administradorServico.BuscaPorId(id);

            if (administrador is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(
                new AdministradorModelView(administrador.Id, administrador.Email, administrador.Perfil));
        })
            .WithName("GetAdministrador")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<AdministradorModelView>(StatusCodes.Status200OK);

        return app;
    }

    private static string GerarTokenJwt(Administrador administrador, string key)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new Claim(ClaimTypes.Email, administrador.Email),
            new Claim(ClaimTypes.Role, administrador.Perfil)
        ];

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credential);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static ErrosDeValidacao ValidaAdministradorDto(AdministradorDto administradorDto)
    {
        List<string> mensagens = [];

        if (string.IsNullOrWhiteSpace(administradorDto.Email))
        {
            mensagens.Add("Email não pode ficar em branco");
        }

        if (string.IsNullOrWhiteSpace(administradorDto.Senha))
        {
            mensagens.Add("Senha não pode ficar em branco");
        }

        if (!Enum.IsDefined(typeof(Perfil), administradorDto.Perfil))
        {
            mensagens.Add("Perfil inválido");
        }

        return new ErrosDeValidacao(mensagens);
    }
}
