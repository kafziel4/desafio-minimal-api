using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options =>
{
    var stringConexao = builder.Configuration.GetConnectionString("mysql");
        
    options.UseMySql(
        stringConexao, ServerVersion.AutoDetect(stringConexao));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDto loginDto) => 
    loginDto is { Email: "adm@test.com", Senha: "123456" }
        ? Results.Ok("Login com sucesso")
        : Results.Unauthorized());

app.Run();
