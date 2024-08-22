using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Infraestrutura.Db;
using Test.Mocks;

namespace Test.Helpers;

public class Setup : WebApplicationFactory<Program>
{
    public HttpClient HttpClient { get; }

    public Setup()
    {
        HttpClient = CreateClient();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DbContexto>>();
            services.RemoveAll<DbContexto>();
            
            services.RemoveAll<IAdministradorServico>();
            services.AddScoped<IAdministradorServico, AdministradorServicoMock>();

            services.RemoveAll<IVeiculoServico>();
            services.AddScoped<IVeiculoServico, VeiculoServicoMock>();
        });
    }

    public async Task AddAuthorizationHeader()
    {
        var loginDto = new LoginDto
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };
        
        var response = await HttpClient.PostAsJsonAsync("/administradores/login", loginDto);
        var admLogado = await response.Content.ReadFromJsonAsync<AdministradorLogado>();

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme, admLogado!.Token);
    }
}