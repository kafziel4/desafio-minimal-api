using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    var stringConexao = builder.Configuration.GetConnectionString("mysql");
        
    options.UseMySql(
        stringConexao, ServerVersion.AutoDetect(stringConexao));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDto loginDto, IAdministradorServico administradorServico) => 
    administradorServico.Login(loginDto) is not null
        ? Results.Ok("Login com sucesso")
        : Results.Unauthorized());

app.Run();
