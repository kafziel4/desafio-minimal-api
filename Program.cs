using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enums;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    var stringConexao = builder.Configuration.GetConnectionString("mysql");
        
    options.UseMySql(
        stringConexao, ServerVersion.AutoDetect(stringConexao));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

#region Home
app.MapGet("/", () => Results.Json(new Home()))
    .WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", (
    [FromBody] LoginDto loginDto,
    IAdministradorServico administradorServico) =>
    administradorServico.Login(loginDto) is not null
        ? Results.Ok("Login com sucesso")
        : Results.Unauthorized())
    .WithTags("Administradores");

app.MapPost("/administradores", (
    [FromBody] AdministradorDto administradorDto,
    IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = []
    };

    if (string.IsNullOrWhiteSpace(administradorDto.Email))
    {
        validacao.Mensagens.Add("Email não pode ficar em branco");
    }
    
    if (string.IsNullOrWhiteSpace(administradorDto.Senha))
    {
        validacao.Mensagens.Add("Senha não pode ficar em branco");
    }
    
    if (!Enum.IsDefined(typeof(Perfil), administradorDto.Perfil))
    {
        validacao.Mensagens.Add("Perfil inválido");
    }

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var administrador = new Administrador
    {
        Email = administradorDto.Email,
        Senha = administradorDto.Senha,
        Perfil = administradorDto.Perfil.ToString()
    };
    
    administradorServico.Incluir(administrador);

    return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var administradores = pagina is > 0
        ? administradorServico.Todos(pagina.Value)
        : administradorServico.Todos();

    var administradoresModelView = administradores
        .Select(a => new AdministradorModelView
        {
            Id = a.Id,
            Email = a.Email,
            Perfil = a.Perfil
        }).ToList();
    
    return Results.Ok(administradoresModelView);
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscaPorId(id);

    if (administrador is null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
}).WithTags("Administradores");
#endregion

#region Veiculos
ErrosDeValidacao ValidaDto(VeiculoDto veiculoDto)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = []
    };

    if (string.IsNullOrWhiteSpace(veiculoDto.Nome))
    {
        validacao.Mensagens.Add("O nome não pode ficar em branco");
    }
    
    if (string.IsNullOrWhiteSpace(veiculoDto.Marca))
    {
        validacao.Mensagens.Add("A marca não pode ficar em branco");
    }
    
    if (veiculoDto.Ano < 1950)
    {
        validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");
    }

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
{
    var validacao = ValidaDto(veiculoDto);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    
    var veiculo = new Veiculo
    {
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano
    };
    
    veiculoServico.Incluir(veiculo);
    
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = pagina is > 0
        ? veiculoServico.Todos(pagina.Value)
        : veiculoServico.Todos();
    
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    
    return veiculo is null ? Results.NotFound() : Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
{
    var validacao = ValidaDto(veiculoDto);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo is null)
    {
        return Results.NotFound();
    }

    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;
    
    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo is null)
    {
        return Results.NotFound();
    }
    
    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculos");
#endregion

app.Run();
